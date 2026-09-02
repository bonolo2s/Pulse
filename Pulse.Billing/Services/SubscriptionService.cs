using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    private readonly IUserLookupService _userLookup;
    private readonly IConfiguration _configuration;


    public SubscriptionService(BillingDbContext context,
        IPaymentProvider paymentProvider,
        IConfiguration configuration,
        IUserLookupService userLookup)
    {
        _context = context;
        _paymentProvider = paymentProvider;
        _configuration = configuration;
        _userLookup = userLookup;
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
        var buffer = TimeSpan.FromHours(_configuration.GetValue<int>("Billing:StuckRenewalBufferHours", 24));

        var expiredSubscriptions = await _context.Subscriptions
            .Where(s => s.IsActive && s.Plan == SubscriptionPlan.Pro && s.ExpiresAt != null && (
                (s.CancelAtPeriodEnd && s.ExpiresAt <= DateTime.UtcNow) || // user cancelled, period ended
                (s.GracePeriodEndsAt != null && s.GracePeriodEndsAt <= DateTime.UtcNow) || // grace period ran out for failed subscriptions.
                (s.GracePeriodEndsAt == null && !s.CancelAtPeriodEnd && s.ExpiresAt <= DateTime.UtcNow - buffer) // renewal never resolved them, verify fallback likely down
            ))
            .ToListAsync();

        foreach (var subscription in expiredSubscriptions)
        {
            subscription.Plan = SubscriptionPlan.Free;
            subscription.ExpiresAt = null;
            subscription.CancelAtPeriodEnd = false;
            subscription.GracePeriodEndsAt = null;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Subscription> GetSubscriptionForRenewalAsync(Guid subscriptionId)
    {
        return await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.Id == subscriptionId && s.IsActive)
            ?? throw new KeyNotFoundException($"Subscription {subscriptionId} not found.");
    }

    public async Task HandleFailedRenewalAsync(Guid subscriptionId)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.Id == subscriptionId && s.IsActive)
            ?? throw new KeyNotFoundException($"Subscription {subscriptionId} not found.");

        if (subscription.GracePeriodEndsAt == null)
        {
            // First failure — start grace period, keep them on Pro
            subscription.GracePeriodEndsAt = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Billing:GracePeriodDays", 3));
        }
        else if (subscription.GracePeriodEndsAt <= DateTime.UtcNow)
        {
            // Grace period exhausted, still failing — downgrade
            subscription.Plan = SubscriptionPlan.Free;
            subscription.ExpiresAt = null;
            subscription.GracePeriodEndsAt = null;
        }

        // else: still within grace, already tracked, nothing new to do — next sweep cycle will retry

        await _context.SaveChangesAsync();
    }

    public async Task ActivateSubscriptionFromWebhookAsync(string email, string subscriptionCode, string emailToken, string customerCode)
    {
        var userId = await _userLookup.GetUserIdByEmailAsync(email)
            ?? throw new KeyNotFoundException($"User with email {email} not found.");

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive)
            ?? throw new KeyNotFoundException($"Subscription for user {userId} not found.");

        subscription.Plan = SubscriptionPlan.Pro;
        subscription.PaystackSubscriptionCode = subscriptionCode;
        subscription.EmailToken = emailToken;
        subscription.PaystackCustomerCode = customerCode;
        subscription.StartedAt = DateTime.UtcNow;
        subscription.ExpiresAt = DateTime.UtcNow.AddMonths(1);

        await _context.SaveChangesAsync();
    }
}