using MediatR;
using Pulse.Billing.Entities;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Queries;

namespace Pulse.Billing.Handlers;

public class GetSubscriptionByCodeHandler : IRequestHandler<GetSubscriptionByCodeQuery, Subscription?>
{
    private readonly ISubscriptionService _subscriptionService;

    public GetSubscriptionByCodeHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public async Task<Subscription?> Handle(GetSubscriptionByCodeQuery request, CancellationToken cancellationToken)
    {
        return await _subscriptionService.GetSubscriptionByCodeAsync(request.SubscriptionCode);
    }
}