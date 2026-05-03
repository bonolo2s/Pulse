using MediatR;

namespace Pulse.Monitoring.Commands;

public record AddEndpointCommand(MonitoredEndpoint Endpoint) : IRequest<MonitoredEndpoint>;