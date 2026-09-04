using MediatR;
using Pulse.Billing.Entities;

namespace Pulse.Billing.Queries;

public record GetSubscriptionByCodeQuery(string SubscriptionCode) : IRequest<Subscription?>;