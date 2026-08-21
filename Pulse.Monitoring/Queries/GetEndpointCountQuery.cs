using MediatR;

namespace Pulse.Monitoring.Queries;

public record GetEndpointCountQuery(Guid UserId) : IRequest<int>;