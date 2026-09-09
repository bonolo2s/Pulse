using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    private readonly decimal _proAmount;

    public BillingService(BillingDbContext context,
        IBillingEventWriter eventWriter,
        IPaymentMethodService paymentMethodService,
        ISubscriptionService subscriptionService,
        IUserLookupService userLookupService,
        IConfiguration configuration)
    {
        _context = context;
        _eventWriter = eventWriter;
        _paymentMethodService = paymentMethodService;
        _subscriptionService = subscriptionService;
        _userLookupService = userLookupService;
        _proAmount = configuration.GetValue<decimal>("Paystack:Plans:Pro");
    }

    public async Task ProcessPaymentResultAsync(string paymentReference,
        string? eventId,
        string status,
        string? channel,
        string? email,
        Guid? userId,
        PaystackAuthorization? authorization)
    {
        if (eventId != null)
        {
            var alreadyProcessed = await _eventWriter.HasProcessedEventAsync(eventId); // coz same payment refernce can fire twice on two sep events.
            if (alreadyProcessed)
            {
                await _eventWriter.LogEventAsync(
                    eventType: BillingEventType.DuplicateEventReceived,
                    source: BillingEventSource.Webhook,
                    userId: null,
                    paystackEventId: eventId,
                    paymentReference: paymentReference,
                    payload: null,
                    previousStatus: null,
                    newStatus: null);
                return;
            }
        }

        Guid resolvedUserId;
        if (userId.HasValue)
        {
            resolvedUserId = userId.Value;
        }
        else if (email != null)
        {
            resolvedUserId = await _userLookupService.GetUserIdByEmailAsync(email)
                ?? throw new KeyNotFoundException($"User with email {email} not found.");
        }
        else
        {
            throw new ArgumentException("Either email or userId must be provided.");
        }

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == resolvedUserId && s.IsActive)
            ?? throw new KeyNotFoundException($"Subscription for user {userId} not found.");

        var parsedStatus = status.ToLowerInvariant() switch
        {
            "success" => PaymentStatus.Successful,
            "failed" => PaymentStatus.Failed,
            _ => throw new InvalidOperationException($"Unrecognized payment status: {status}")
        };

        var newEventType = parsedStatus switch
        {
            PaymentStatus.Successful => BillingEventType.PaymentSuccessful,
            PaymentStatus.Failed => BillingEventType.PaymentFailed,
            _ => BillingEventType.Unknown
        };

        var lastEvent = await _context.BillingEvents
            .Where(e => e.PaymentReference == paymentReference)
            .OrderByDescending(e => e.ReceivedAt)
            .FirstOrDefaultAsync();

        var previousStatus = lastEvent?.NewStatus;

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
                Amount = _proAmount,
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
            Provider = "Paystack",
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
            eventType: newEventType,
            source: BillingEventSource.Webhook,
            userId: subscription.UserId,
            paystackEventId: eventId,
            paymentReference: paymentReference,
            payload: null,
            previousStatus: previousStatus,
            newStatus: newEventType);
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
            //Status = PaymentStatus.Pending,
            //Method = PaymentMethodType.Card,
            Provider = "Paystack",
            ProviderReference = providerReference,
            //CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return payment;
    }
}