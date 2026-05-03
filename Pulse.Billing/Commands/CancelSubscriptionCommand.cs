using MediatR;

namespace Pulse.Billing.Commands;

public record CancelSubscriptionCommand(Guid UserId) : IRequest;