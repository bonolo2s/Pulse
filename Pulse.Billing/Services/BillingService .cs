using Microsoft.EntityFrameworkCore;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Entities;
using Pulse.Billing.Enums;
using Pulse.Billing.Interfaces;
using Pulse.Shared.Interfaces;

namespace Pulse.Billing.Services;

public class BillingService : IBillingService, IBillingValidator, ISubscriptionCreator
{
    private readonly BillingDbContext _context;
    private readonly IBillingEventWriter _eventWriter;

    public BillingService(BillingDbContext context, IBillingEventWriter eventWriter)
    {
        _context = context;
        _eventWriter = eventWriter;
    }

    public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription) // Not a conflict .Admin might need it 
    {
        subscription.Id = Guid.NewGuid();
        subscription.Plan = SubscriptionPlan.Free;
        subscription.EndpointLimit = 3;
        subscription.StartedAt = DateTime.UtcNow;
        subscription.IsActive = true;

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        return subscription;
    }
    public async Task CreateSubscriptionAsync(Guid userId) // used by auth on Register
    {
        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Plan = SubscriptionPlan.Free,
            EndpointLimit = 3,
            StartedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
    }

    public async Task<Subscription> UpgradeToProAsync(Guid userId)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive)
            ?? throw new KeyNotFoundException($"Subscription for user {userId} not found.");

        // RESERVED: only call this after Paystack payment is confirmed (webhook or verify fallback).
        // TODO: add fault tolerance here — retry on transient DB failure before returning success to caller.

        subscription.Plan = SubscriptionPlan.Pro;
        subscription.EndpointLimit = int.MaxValue;
        subscription.StartedAt = DateTime.UtcNow;
        subscription.ExpiresAt = DateTime.UtcNow.AddMonths(1);

        await _context.SaveChangesAsync();
        return subscription;
    }

    // TODO: VerifySubscriptionUpgradeAsync — fallback path for when webhook doesn't arrive (Ghost webhook)/ after cetain time

    public async Task CancelSubscriptionAsync(Guid userId)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive)
            ?? throw new KeyNotFoundException($"Subscription for user {userId} not found.");

        // TODO: call IPaymentProvider.DisableSubscription to stop future Paystack billing
        // TODO: don't cut access immediately — let ExpiresAt (already paid period) run out, only flip IsActive when it lapses
        subscription.IsActive = false;
        subscription.ExpiresAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<Subscription> GetSubscriptionAsync(Guid userId)
    {
        return await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive)
            ?? throw new KeyNotFoundException($"Subscription for user {userId} not found.");
    }

    public async Task<IEnumerable<Invoice>> GetBillingHistoryAsync(Guid userId)
    {
        return await _context.Invoices
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.IssuedAt)
            .ToListAsync();
    }

    public async Task ProcessPaymentResultAsync(string paymentReference, string status) //**
    {
        var alreadyProcessed = await _eventWriter.HasProcessedEventAsync(paymentReference);
        if (alreadyProcessed)
        {
            await _eventWriter.LogEventAsync(
                eventType: BillingEventType.DuplicateEventReceived,
                source: BillingEventSource.Webhook,
                paymentId: null,
                userId: null,
                paystackEventId: paymentReference,
                payload: null,
                previousStatus: null,
                newStatus: null);
            return;
        }

        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.ProviderReference == paymentReference)
            ?? throw new KeyNotFoundException($"Payment with reference {paymentReference} not found.");

        // TODO: idempotency check — look up BillingEvent by PaystackEventId, skip if already Processed

        var parsedStatus = status.ToLowerInvariant() switch
        {
            "success" => PaymentStatus.Successful,
            "failed" => PaymentStatus.Failed,
            "pending" or "processing" => PaymentStatus.Processing,
            _ => throw new InvalidOperationException($"Unrecognized payment status: {status}")
        };

        var previousStatus = payment.Status.ToString();

        payment.Status = parsedStatus;
        payment.CompletedAt = DateTime.UtcNow;

        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == payment.InvoiceId)
            ?? throw new KeyNotFoundException($"Invoice {payment.InvoiceId} not found.");

        if (parsedStatus == PaymentStatus.Successful)
        {
            invoice.Status = InvoiceStatus.Paid;
            invoice.PaidAt = DateTime.UtcNow;

            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.Id == invoice.SubscriptionId)
                ?? throw new KeyNotFoundException($"Subscription {invoice.SubscriptionId} not found.");

            subscription.Plan = SubscriptionPlan.Pro;
            subscription.EndpointLimit = int.MaxValue;
            subscription.ExpiresAt = DateTime.UtcNow.AddMonths(1);
        }

        await _context.SaveChangesAsync();

        await _eventWriter.LogEventAsync(
            eventType: parsedStatus == PaymentStatus.Successful ? BillingEventType.PaymentSuccessful : BillingEventType.PaymentFailed,
            source: BillingEventSource.Webhook,
            paymentId: payment.Id,
            userId: payment.UserId,
            paystackEventId: paymentReference,
            payload: null,
            previousStatus: previousStatus,
            newStatus: parsedStatus.ToString());
    }

    public async Task ValidateEndpointLimitAsync(Guid userId, int currentEndpointCount)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive)
            ?? throw new KeyNotFoundException($"Subscription for user {userId} not found.");

        if (currentEndpointCount >= subscription.EndpointLimit)
            throw new InvalidOperationException($"Endpoint limit of {subscription.EndpointLimit} reached. Go Pro for unlimited monitoring.");
    }

    public async Task<Invoice> CreatePendingInvoiceAsync(Guid userId, Guid subscriptionId, decimal amount, string currency)
    {
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SubscriptionId = subscriptionId,
            Amount = amount,
            Currency = currency,
            Status = InvoiceStatus.Pending,
            IssuedAt = DateTime.UtcNow
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        return invoice;
    }

    public async Task<Payment> CreatePendingPaymentAsync(Guid userId, Guid invoiceId, decimal amount, string providerReference)
    {
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            InvoiceId = invoiceId,
            Amount = amount,
            Status = PaymentStatus.Pending,
            //Method = PaymentMethodType.Card,
            Provider = "Paystack",
            ProviderReference = providerReference,
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return payment;
    }
}