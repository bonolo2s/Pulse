using MediatR;
using Microsoft.EntityFrameworkCore;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Entities;
using Pulse.Billing.Enums;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Payments.Interfaces;
using Pulse.Billing.Payments.Paystack.DTOs;
using Pulse.Shared.Interfaces;

namespace Pulse.Billing.Services;

public class SubscriptionService : ISubscriptionService, ISubscriptionCreator
{
    private readonly BillingDbContext _context;
    private readonly IPaymentProvider _paymentProvider;

    public SubscriptionService(BillingDbContext context, IPaymentProvider paymentProvider)
    {
        _context = context;
        _paymentProvider = paymentProvider;
    }

    public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription) // Not a conflict .Admin might need it 
    {
        subscription.Id = Guid.NewGuid();
        subscription.Plan = SubscriptionPlan.Free;
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

        subscription.CancelAtPeriodEnd = true;
        await _context.SaveChangesAsync();
    }

    public async Task<Subscription> GetSubscriptionAsync(Guid userId)
    {
        return await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive)
            ?? throw new KeyNotFoundException($"Subscription for user {userId} not found.");
    }

    public async Task ProcessExpiredSubscriptionsAsync()
    {
        var expiredSubscriptions = await _context.Subscriptions
            .Where(s => s.IsActive && s.CancelAtPeriodEnd && s.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync();

        foreach (var subscription in expiredSubscriptions)
        {
            subscription.Plan = SubscriptionPlan.Free;
            subscription.ExpiresAt = null;
            subscription.CancelAtPeriodEnd = false;
        }

        await _context.SaveChangesAsync();
    }

    public async Task RenewSubscriptionAsync(Guid subscriptionId, string email)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.Id == subscriptionId && s.IsActive)
            ?? throw new KeyNotFoundException($"Subscription {subscriptionId} not found.");

        var paymentMethod = await _context.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.UserId == subscription.UserId && pm.IsDefault)
            ?? throw new InvalidOperationException($"No default payment method for user {subscription.UserId}.");

        var result = await _paymentProvider.ChargeAuthorization(new ChargeAuthorizationRequest(
            Email: email,
            Amount: subscription.MonthlyPrice,
            AuthorizationCode: paymentMethod.AuthorizationCode
        ));

        var invoice = await _invoiceService.CreatePendingInvoiceAsync(request.UserId, subscription.Id, amount, currency);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            UserId = subscription.UserId,
            InvoiceId = /* ? */,
            Amount = subscription.MonthlyPrice,
            Status = PaymentStatus.Pending,
            Provider = "Paystack",
            ProviderReference = result.Reference,
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
    }

    public async Task<Subscription> GetSubscriptionForRenewalAsync(Guid subscriptionId)
    {
        return await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.Id == subscriptionId && s.IsActive)
            ?? throw new KeyNotFoundException($"Subscription {subscriptionId} not found.");
    }
}