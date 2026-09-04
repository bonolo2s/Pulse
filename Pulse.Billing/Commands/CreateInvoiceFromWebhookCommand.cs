using MediatR;

namespace Pulse.Billing.Commands;

public record CreateInvoiceFromWebhookCommand(
    string SubscriptionCode,
    string InvoiceCode,
    int Amount,
    string Currency,
    string EmailToken) : IRequest;