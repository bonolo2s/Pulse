using MediatR;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Queries;

namespace Pulse.Billing.Handlers;

public class GetSubscriptionHandler : IRequestHandler<GetSubscriptionQuery, Subscription>
{
    private readonly IBillingService _billingService;

    public GetSubscriptionHandler(IBillingService billingService)
    {
        _billingService = billingService;
    }

    public async Task<Subscription> Handle(GetSubscriptionQuery request, CancellationToken cancellationToken)
    {
        return await _billingService.GetSubscriptionAsync(request.UserId);
    }
}