using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Handlers;

public class UpdateInvoiceFromWebhookHandler : IRequestHandler<UpdateInvoiceFromWebhookCommand>
{
    private readonly IInvoiceService _invoiceService;

    public UpdateInvoiceFromWebhookHandler(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public async Task Handle(UpdateInvoiceFromWebhookCommand request, CancellationToken cancellationToken)
    {
        await _invoiceService.UpdateInvoiceFromWebhookAsync(request.InvoiceCode, request.Status, request.Paid);
    }
}