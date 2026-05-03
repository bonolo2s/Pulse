using MediatR;
using Pulse.Observability.Entities;
using Pulse.Observability.Interfaces;
using Pulse.Observability.Queries;

namespace Pulse.Observability.Handlers;

public class GetUptimeHistoryHandler : IRequestHandler<GetUptimeHistoryQuery, IEnumerable<CheckResult>>
{
    private readonly IObservabilityService _observabilityService;

    public GetUptimeHistoryHandler(IObservabilityService observabilityService)
    {
        _observabilityService = observabilityService;
    }

    public async Task<IEnumerable<CheckResult>> Handle(GetUptimeHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _observabilityService.GetUptimeHistoryAsync(request.EndpointId, request.Days);
    }
}