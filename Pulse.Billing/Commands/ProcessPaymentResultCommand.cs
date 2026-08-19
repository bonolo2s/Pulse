using MediatR;

namespace Pulse.Billing.Commands;

public record ProcessPaymentResultCommand(string PaymentReference, string Status) : IRequest;