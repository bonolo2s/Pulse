using Microsoft.EntityFrameworkCore;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Entities;
using Pulse.Billing.Interfaces;
using Pulse.Shared.Interfaces;

namespace Pulse.Billing.Services;

public class BillingService : IBillingService, IBillingValidator, ISubscriptionCreator
{
    private readonly BillingDbContext _context;

    public BillingService(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription)
    {
        subscription.Id = Guid.NewGuid();
        subscription.Plan = "Free";
        subscription.EndpointLimit = 3;
        subscription.StartedAt = DateTime.UtcNow;
        subscription.IsActive = true;

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        return subscription;
    }

    public async Task<Subscription> UpgradeToProAsync(Guid userId)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive)
            ?? throw new KeyNotFoundException($"Subscription for user {userId} not found.");

        subscription.Plan = "Pro";
        subscription.EndpointLimit = int.MaxValue;
        subscription.StartedAt = DateTime.UtcNow;
        subscription.ExpiresAt = DateTime.UtcNow.AddMonths(1);

        await _context.SaveChangesAsync();

        return subscription;
    }

    public async Task CancelSubscriptionAsync(Guid userId)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive)
            ?? throw new KeyNotFoundException($"Subscription for user {userId} not found.");

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

    public async Task SyncPaymentWebhookAsync(string paymentReference, string status)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.PaymentReference == paymentReference)
            ?? throw new KeyNotFoundException($"Invoice with reference {paymentReference} not found.");

        invoice.Status = Enum.Parse<InvoiceStatus>(status, ignoreCase: true);

        if (invoice.Status == InvoiceStatus.Paid)
        {
            invoice.PaidAt = DateTime.UtcNow;

            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.Id == invoice.SubscriptionId)
                ?? throw new KeyNotFoundException($"Subscription {invoice.SubscriptionId} not found.");

            subscription.Plan = "Pro";
            subscription.EndpointLimit = int.MaxValue;
            subscription.ExpiresAt = DateTime.UtcNow.AddMonths(1);
        }

        await _context.SaveChangesAsync();
    }

    public async Task ValidateEndpointLimitAsync(Guid userId, int currentEndpointCount)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive)
            ?? throw new KeyNotFoundException($"Subscription for user {userId} not found.");

        if (currentEndpointCount >= subscription.EndpointLimit)
            throw new InvalidOperationException($"Endpoint limit of {subscription.EndpointLimit} reached. Upgrade to Pro.");
    }

    public async Task CreateSubscriptionAsync(Guid userId)
    {
        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Plan = "Free",
            EndpointLimit = 3,
            StartedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
    }
}