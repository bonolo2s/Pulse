using Pulse.Billing.Interfaces;

namespace Pulse.Billing.BackgroundServices;

public class SubscriptionExpirySweepService : ISubscriptionExpirySweepService
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionExpirySweepService(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public async Task SweepAsync(CancellationToken cancellationToken)
    {
        await _subscriptionService.ProcessExpiredSubscriptionsAsync();
    }
}