using MediatR;

namespace Pulse.Monitoring.Queries;

public record GetEndpointsQuery(Guid UserId) : IRequest<IEnumerable<MonitoredEndpoint>>;