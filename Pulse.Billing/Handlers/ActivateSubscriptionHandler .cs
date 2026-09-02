using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Handlers;

public class ActivateSubscriptionHandler : IRequestHandler<ActivateSubscriptionCommand>
{
    private readonly ISubscriptionService _subscriptionService;

    public ActivateSubscriptionHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public async Task Handle(ActivateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        await _subscriptionService.ActivateSubscriptionFromWebhookAsync(
            request.Email, request.SubscriptionCode, request.EmailToken, request.CustomerCode);
    }
}