using MediatR;

namespace Pulse.Billing.Commands;

public record UpdateInvoiceFromWebhookCommand(
    string InvoiceCode,
    string Status,
    bool Paid) : IRequest;