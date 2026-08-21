using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.Entities;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Handlers;

public class CreateSubscriptionHandler : IRequestHandler<CreateSubscriptionCommand, Subscription>
{
    private readonly ISubscriptionService _subscriptionService;

    public CreateSubscriptionHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public async Task<Subscription> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        return await _subscriptionService.CreateSubscriptionAsync(request.Subscription);
    }
}