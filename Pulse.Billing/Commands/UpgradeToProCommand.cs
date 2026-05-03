using MediatR;
using Pulse.Billing.Entities;

namespace Pulse.Billing.Commands;

public record UpgradeToProCommand(Guid UserId) : IRequest<Subscription>;