using MediatR;

namespace Pulse.Billing.Commands;

public record SetDefaultPaymentMethodCommand(Guid Id) : IRequest;