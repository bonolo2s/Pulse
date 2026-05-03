using MediatR;
using Pulse.Billing.Entities;

namespace Pulse.Billing.Queries;

public record GetBillingHistoryQuery(Guid UserId) : IRequest<IEnumerable<Invoice>>;