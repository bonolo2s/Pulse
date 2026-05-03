using MediatR;
using Pulse.Monitoring.Commands;
using Pulse.Monitoring.Interfaces;

namespace Pulse.Monitoring.Handlers;

public class EditEndpointHandler : IRequestHandler<EditEndpointCommand, MonitoredEndpoint>
{
    private readonly IMonitoringService _monitoringService;

    public EditEndpointHandler(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    public async Task<MonitoredEndpoint> Handle(EditEndpointCommand request, CancellationToken cancellationToken)
    {
        return await _monitoringService.EditEndpointAsync(request.Id, request.Endpoint);
    }
}