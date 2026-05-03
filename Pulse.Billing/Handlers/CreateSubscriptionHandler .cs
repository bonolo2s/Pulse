using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.Entities;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Handlers;

public class CreateSubscriptionHandler : IRequestHandler<CreateSubscriptionCommand, Subscription>
{
    private readonly IBillingService _billingService;

    public CreateSubscriptionHandler(IBillingService billingService)
    {
        _billingService = billingService;
    }

    public async Task<Subscription> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        return await _billingService.CreateSubscriptionAsync(request.Subscription);
    }
}