using MediatR;
using Pulse.Monitoring.Interfaces;
using Pulse.Monitoring.Queries;

namespace Pulse.Monitoring.Handlers;

public class GetEndpointCountHandler : IRequestHandler<GetEndpointCountQuery, int>
{
    private readonly IMonitoringService _monitoringService;

    public GetEndpointCountHandler(IMonitoringService monitoringService)
    {
        _monitoringService = monitoringService;
    }

    public async Task<int> Handle(GetEndpointCountQuery request, CancellationToken cancellationToken)
    {
        return await _monitoringService.GetEndpointCountAsync(request.UserId);
    }
}