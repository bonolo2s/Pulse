using MediatR;
using Pulse.Billing.Entities;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Queries;

namespace Pulse.Billing.Handlers;

public class GetSubscriptionHandler : IRequestHandler<GetSubscriptionQuery, Subscription>
{
    private readonly ISubscriptionService _subscriptionService;

    public GetSubscriptionHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public async Task<Subscription> Handle(GetSubscriptionQuery request, CancellationToken cancellationToken)
    {
        return await _subscriptionService.GetSubscriptionAsync(request.UserId);
    }
}