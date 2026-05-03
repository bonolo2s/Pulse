using MediatR;
using Pulse.Billing.Entities;

namespace Pulse.Billing.Commands;

public record CreateSubscriptionCommand(Subscription Subscription) : IRequest<Subscription>;