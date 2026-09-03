using MediatR;
using Pulse.Billing.Commands;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Handlers;

public class CreateInvoiceFromWebhookHandler : IRequestHandler<CreateInvoiceFromWebhookCommand>
{
    private readonly IInvoiceService _invoiceService;

    public CreateInvoiceFromWebhookHandler(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public async Task Handle(CreateInvoiceFromWebhookCommand request, CancellationToken cancellationToken)
    {
        await _invoiceService.CreateInvoiceFromWebhookAsync(
            request.SubscriptionCode,
            request.InvoiceCode,
            request.Amount,
            request.Currency,
            request.Status,
            request.Paid,
            request.PaidAt);
    }
}