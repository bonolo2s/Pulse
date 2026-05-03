using MediatR;
using Pulse.Monitoring.Commands;
using Pulse.Monitoring.Interfaces;

namespace Pulse.Monitoring.Handlers;

public class RemoveEndpointHandler : IRequestHandler<RemoveEndpointCommand>
{
    private readonly IMonitoringService _monitoringService;

    public RemoveEndpointHandler(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    public async Task Handle(RemoveEndpointCommand request, CancellationToken cancellationToken)
    {
        await _monitoringService.RemoveEndpointAsync(request.Id);
    }
}