using MediatR;

namespace Pulse.Billing.Commands;

public record SyncPaymentWebhookCommand(string PaymentReference, string Status) : IRequest;