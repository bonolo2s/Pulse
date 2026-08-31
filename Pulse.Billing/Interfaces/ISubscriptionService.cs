using Pulse.Billing.Entities;

namespace Pulse.Billing.Interfaces;

public interface ISubscriptionService
{
    Task<Subscription> CreateSubscriptionAsync(Subscription subscription);
    Task<Subscription> UpgradeToProAsync(Guid userId);
    Task CancelSubscriptionAsync(Guid userId);
    Task<Subscription> GetSubscriptionAsync(Guid userId);
    Task ProcessExpiredSubscriptionsAsync();
    Task<Subscription> GetSubscriptionForRenewalAsync(Guid subscriptionId);
    Task HandleFailedRenewalAsync(Guid subscriptionId);
}