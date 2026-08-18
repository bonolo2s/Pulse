using Pulse.Billing.Entities;
using System.Reflection.Metadata;

namespace Pulse.Billing.Interfaces;

public interface IBillingService
{
    Task<Subscription> CreateSubscriptionAsync(Subscription subscription);
    Task<Subscription> UpgradeToProAsync(Guid userId);
    Task CancelSubscriptionAsync(Guid userId);
    Task<Subscription> GetSubscriptionAsync(Guid userId);
    Task<IEnumerable<Invoice>> GetBillingHistoryAsync(Guid userId);
    Task ProcessPaymentResultAsync(string paymentReference, string status);
}

//RenewSubscriptionAsync — handles the recurring charge webhook event (Paystack subscription.create/charge.success on renewal), extends ExpiresAt again
//HandleFailedRenewalAsync — recurring charge fails (card declined etc.) — grace period vs immediate downgrade decision
//DowngradeExpiredSubscriptionsAsync — background sweep for subscriptions where ExpiresAt has passed and no renewal came in, flips them back to Free