using Pulse.Billing.Entities;

namespace Pulse.Billing.Interfaces;

public interface ISubscriptionService
{
    Task<Subscription> CreateSubscriptionAsync(Subscription subscription);
    Task<Subscription> UpgradeToProAsync(Guid userId);
    Task CancelSubscriptionAsync(Guid userId);
    Task<Subscription> GetSubscriptionAsync(Guid userId);
    //Task ProcessExpiredSubscriptionsAsync();
    Task DowngradeSubscriptionFromWebhookAsync(string subscriptionCode);
    Task<Subscription> GetSubscriptionForRenewalAsync(Guid subscriptionId);
    Task<Subscription?> GetSubscriptionByCodeAsync(string subscriptionCode);
    Task HandleFailedRenewalAsync(Guid subscriptionId);
    Task ActivateSubscriptionFromWebhookAsync(string email, string subscriptionCode, string emailToken, string customerCode);
}