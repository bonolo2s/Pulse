using MediatR;

namespace Pulse.Billing.Commands;

public record DeletePaymentMethodCommand(Guid Id) : IRequest;