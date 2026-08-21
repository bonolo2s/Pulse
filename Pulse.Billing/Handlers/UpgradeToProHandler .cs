using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.Entities;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Handlers;

public class UpgradeToProHandler : IRequestHandler<UpgradeToProCommand, Subscription>
{
    private readonly ISubscriptionService _subscriptionService;

    public UpgradeToProHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public async Task<Subscription> Handle(UpgradeToProCommand request, CancellationToken cancellationToken)
    {
        return await _subscriptionService.UpgradeToProAsync(request.UserId);
    }
}