using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Handlers;

public class CancelSubscriptionHandler : IRequestHandler<CancelSubscriptionCommand>
{
    private readonly IBillingService _billingService;

    public CancelSubscriptionHandler(IBillingService billingService)
    {
        _billingService = billingService;
    }

    public async Task Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        await _billingService.CancelSubscriptionAsync(request.UserId);
    }
}