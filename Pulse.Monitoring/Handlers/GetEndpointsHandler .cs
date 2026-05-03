using MediatR;
using Pulse.Monitoring.Interfaces;
using Pulse.Monitoring.Queries;

namespace Pulse.Monitoring.Handlers;

public class GetEndpointsHandler : IRequestHandler<GetEndpointsQuery, IEnumerable<MonitoredEndpoint>>
{
    private readonly IMonitoringService _monitoringService;

    public GetEndpointsHandler(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    public async Task<IEnumerable<MonitoredEndpoint>> Handle(GetEndpointsQuery request, CancellationToken cancellationToken)
    {
        return await _monitoringService.GetEndpointsAsync(request.UserId);
    }
}