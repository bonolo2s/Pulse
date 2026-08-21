using Microsoft.EntityFrameworkCore;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Entities;
using Pulse.Billing.Enums;
using Pulse.Billing.Interfaces;
using Pulse.Shared.Interfaces;

namespace Pulse.Billing.Services;

public class SubscriptionService : ISubscriptionService, ISubscriptionCreator
{
    private readonly BillingDbContext _context;

    public SubscriptionService(BillingDbContext context)
    {
        _context = context;
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
}