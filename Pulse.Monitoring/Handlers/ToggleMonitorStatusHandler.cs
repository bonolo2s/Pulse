using MediatR;
using Pulse.Monitoring.Commands;
using Pulse.Monitoring.Interfaces;

namespace Pulse.Monitoring.Handlers;

public class ToggleMonitorStatusHandler : IRequestHandler<ToggleMonitorStatusCommand>
{
    private readonly IMonitoringService _monitoringService;

    public ToggleMonitorStatusHandler(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    public async Task Handle(ToggleMonitorStatusCommand request, CancellationToken cancellationToken)
    {
        await _monitoringService.ToggleMonitorStatusAsync(request.Id);
    }
}