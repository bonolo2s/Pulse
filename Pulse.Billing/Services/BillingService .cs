using Microsoft.EntityFrameworkCore;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Entities;
using Pulse.Billing.Enums;
using Pulse.Billing.Interfaces;
using Pulse.Shared.Interfaces;

namespace Pulse.Billing.Services;

public class BillingService : IBillingService, IBillingValidator
{
    private readonly BillingDbContext _context;
    private readonly IBillingEventWriter _eventWriter;

    public BillingService(BillingDbContext context, IBillingEventWriter eventWriter)
    {
        _context = context;
        _eventWriter = eventWriter;
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