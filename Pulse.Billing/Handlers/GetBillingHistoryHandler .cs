using MediatR;
using Pulse.Billing.Entities;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Queries;

namespace Pulse.Billing.Handlers;

public class GetBillingHistoryHandler : IRequestHandler<GetBillingHistoryQuery, IEnumerable<Invoice>>
{
    private readonly IInvoiceService _invoiceService;

    public GetBillingHistoryHandler(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public async Task<IEnumerable<Invoice>> Handle(GetBillingHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _invoiceService.GetBillingHistoryAsync(request.UserId);
    }
}