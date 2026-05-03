using Pulse.Billing.Entities;

namespace Pulse.Billing.Interfaces;

public interface IBillingService
{
    Task<Subscription> CreateSubscriptionAsync(Subscription subscription);
    Task<Subscription> UpgradeToProAsync(Guid userId);
    Task CancelSubscriptionAsync(Guid userId);
    Task<Subscription> GetSubscriptionAsync(Guid userId);
    Task<IEnumerable<Invoice>> GetBillingHistoryAsync(Guid userId);
    Task SyncPaymentWebhookAsync(string paymentReference, string status);
}