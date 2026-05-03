using MediatR;
using Pulse.Monitoring.Commands;
using Pulse.Monitoring.Interfaces;

namespace Pulse.Monitoring.Handlers;

public class AddEndpointHandler : IRequestHandler<AddEndpointCommand, MonitoredEndpoint>
{
    private readonly IMonitoringService _monitoringService;

    public AddEndpointHandler(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    public async Task<MonitoredEndpoint> Handle(AddEndpointCommand request, CancellationToken cancellationToken)
    {
        return await _monitoringService.AddEndpointAsync(request.Endpoint);
    }
}