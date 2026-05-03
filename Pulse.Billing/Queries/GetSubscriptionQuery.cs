using MediatR;
using Pulse.Billing.Entities;

namespace Pulse.Billing.Queries;

public record GetSubscriptionQuery(Guid UserId) : IRequest<Subscription>;