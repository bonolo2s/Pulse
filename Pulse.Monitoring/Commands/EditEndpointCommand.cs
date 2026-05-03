using MediatR;

namespace Pulse.Monitoring.Commands;

public record EditEndpointCommand(Guid Id, MonitoredEndpoint Endpoint) : IRequest<MonitoredEndpoint>;