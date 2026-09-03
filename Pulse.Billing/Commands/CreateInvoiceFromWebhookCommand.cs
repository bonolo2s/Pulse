using MediatR;

namespace Pulse.Billing.Commands;

public record CreateInvoiceFromWebhookCommand(
    string SubscriptionCode,
    string InvoiceCode,
    int Amount,
    string Currency,
    string Status,
    bool Paid,
    DateTime? PaidAt) : IRequest;