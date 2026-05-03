using MediatR;
using Pulse.Observability.Entities;

namespace Pulse.Observability.Queries;

public record GetLatencyTrendsQuery(Guid EndpointId, int Days) : IRequest<IEnumerable<CheckResult>>;