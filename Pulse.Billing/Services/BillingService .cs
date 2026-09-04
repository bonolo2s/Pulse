using Microsoft.EntityFrameworkCore;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Entities;
using Pulse.Billing.Enums;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Payments.Paystack.DTOs;
using Pulse.Shared.Interfaces;

namespace Pulse.Billing.Services;

public class BillingService : IBillingService, IBillingValidator
{
    private readonly BillingDbContext _context;
    private readonly IBillingEventWriter _eventWriter;
    private readonly IPaymentMethodService _paymentMethodService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IUserLookupService _userLookupService;
    public BillingService(BillingDbContext context,
        IBillingEventWriter eventWriter,
        IPaymentMethodService paymentMethodService,
        ISubscriptionService subscriptionService,
        IUserLookupService userLookupService)
    {
        _context = context;
        _eventWriter = eventWriter;
        _paymentMethodService = paymentMethodService;
        _subscriptionService = subscriptionService;
        _userLookupService = userLookupService;
    }

    public async Task ProcessPaymentResultAsync(string paymentReference, string eventId, string status, string? channel, string email, PaystackAuthorization? authorization)
    {
        var alreadyProcessed = await _eventWriter.HasProcessedEventAsync(eventId); // coz same payment refernce can fire twice on two sep events
        if (alreadyProcessed)
        {
            await _eventWriter.LogEventAsync(
                eventType: BillingEventType.DuplicateEventReceived,
                source: BillingEventSource.Webhook,
                paymentId: null,
                userId: null,
                paystackEventId: eventId,
                payload: null,
                previousStatus: null,
                newStatus: null);
            return;
        }

        var userId = await _userLookupService.GetUserIdByEmailAsync(email)
            ?? throw new KeyNotFoundException($"User with email {email} not found.");

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive)
            ?? throw new KeyNotFoundException($"Subscription for user {userId} not found.");

        var parsedStatus = status.ToLowerInvariant() switch
        {
            "success" => PaymentStatus.Successful,
            "failed" => PaymentStatus.Failed,
            "pending" or "processing" => PaymentStatus.Processing,
            _ => throw new InvalidOperationException($"Unrecognized payment status: {status}")
        };

        var invoice = await _context.Invoices
            .Where(i => i.SubscriptionId == subscription.Id && !_context.Payments.Any(p => p.InvoiceId == i.Id))
            .OrderByDescending(i => i.IssuedAt)
            .FirstOrDefaultAsync();

        bool isFirstPayment = invoice == null;

        if (isFirstPayment)
        {
            invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                UserId = subscription.UserId,
                SubscriptionId = subscription.Id,
                Amount = subscription.MonthlyPrice,
                Currency = "ZAR",
                Status = parsedStatus == PaymentStatus.Successful ? InvoiceStatus.Success : InvoiceStatus.Failed,
                Type = InvoiceType.Initial,
                IssuedAt = DateTime.UtcNow,
                PaidAt = parsedStatus == PaymentStatus.Successful ? DateTime.UtcNow : null
            };
            _context.Invoices.Add(invoice);
        }

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = subscription.UserId,
            InvoiceId = invoice!.Id,
            Amount = invoice.Amount,
            ProviderReference = paymentReference,
            Status = parsedStatus,
            CompletedAt = DateTime.UtcNow
        };
        _context.Payments.Add(payment);

        if (parsedStatus == PaymentStatus.Successful)
        {
            if (!isFirstPayment)
            {
                subscription.ExpiresAt = DateTime.UtcNow.AddMonths(1);
            }
            if (isFirstPayment)
            {
                subscription.Plan = SubscriptionPlan.Pro;
                subscription.ExpiresAt = DateTime.UtcNow.AddMonths(1);
            }
            if (authorization != null && authorization.Reusable)
            {
                var paymentMethod = new PaymentMethod
                {
                    UserId = subscription.UserId,
                    Type = channel == "card" ? PaymentMethodType.Card : PaymentMethodType.Eft,
                    AuthorizationCode = authorization.AuthorizationCode
                };

                if (channel == "card")
                {
                    paymentMethod.Brand = Enum.TryParse<CardBrand>(authorization.CardType, true, out var brand) ? brand : null;
                    paymentMethod.Last4 = authorization.Last4;
                    paymentMethod.ExpiryMonth = int.TryParse(authorization.ExpMonth, out var m) ? m : null;
                    paymentMethod.ExpiryYear = int.TryParse(authorization.ExpYear, out var y) ? y : null;
                }
                else
                {
                    paymentMethod.BankName = authorization.Bank;
                }

                await _paymentMethodService.SavePaymentMethodAsync(paymentMethod);
            }
        }
        else if (parsedStatus == PaymentStatus.Failed && !isFirstPayment)
        {
            await _subscriptionService.HandleFailedRenewalAsync(invoice.SubscriptionId);
        }

        await _context.SaveChangesAsync();

        await _eventWriter.LogEventAsync(
            eventType: parsedStatus == PaymentStatus.Successful ? BillingEventType.PaymentSuccessful : BillingEventType.PaymentFailed,
            source: BillingEventSource.Webhook,
            paymentId: payment.Id,
            userId: subscription.UserId,
            paystackEventId: eventId,
            payload: null,
            previousStatus: null,
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