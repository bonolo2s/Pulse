using MediatR;
using Pulse.Billing.Entities;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Queries;

namespace Pulse.Billing.Handlers;

public class GetBillingHistoryHandler : IRequestHandler<GetBillingHistoryQuery, IEnumerable<Invoice>>
{
    private readonly IBillingService _billingService;

    public GetBillingHistoryHandler(IBillingService billingService)
    {
        _billingService = billingService;
    }

    public async Task<IEnumerable<Invoice>> Handle(GetBillingHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _billingService.GetBillingHistoryAsync(request.UserId);
    }
}