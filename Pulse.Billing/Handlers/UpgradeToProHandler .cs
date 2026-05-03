using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Handlers;

public class UpgradeToProHandler : IRequestHandler<UpgradeToProCommand, Subscription>
{
    private readonly IBillingService _billingService;

    public UpgradeToProHandler(IBillingService billingService)
    {
        _billingService = billingService;
    }

    public async Task<Subscription> Handle(UpgradeToProCommand request, CancellationToken cancellationToken)
    {
        return await _billingService.UpgradeToProAsync(request.UserId);
    }
}