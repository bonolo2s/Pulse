using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Handlers;

public class CancelSubscriptionHandler : IRequestHandler<CancelSubscriptionCommand>
{
    private readonly ISubscriptionService _subscriptionService;

    public CancelSubscriptionHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public async Task Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        await _subscriptionService.CancelSubscriptionAsync(request.UserId);
    }
}