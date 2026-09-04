using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.Interfaces;

public class DowngradeSubscriptionHandler : IRequestHandler<DowngradeSubscriptionCommand>
{
    private readonly ISubscriptionService _subscriptionService;

    public DowngradeSubscriptionHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public async Task Handle(DowngradeSubscriptionCommand request, CancellationToken cancellationToken)
    {
        await _subscriptionService.DowngradeSubscriptionFromWebhookAsync(request.SubscriptionCode);
    }
}