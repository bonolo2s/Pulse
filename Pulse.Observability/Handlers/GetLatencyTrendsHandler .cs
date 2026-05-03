using MediatR;
using Pulse.Observability.Entities;
using Pulse.Observability.Interfaces;
using Pulse.Observability.Queries;

namespace Pulse.Observability.Handlers;

public class GetLatencyTrendsHandler : IRequestHandler<GetLatencyTrendsQuery, IEnumerable<CheckResult>>
{
    private readonly IObservabilityService _observabilityService;

    public GetLatencyTrendsHandler(IObservabilityService observabilityService)
    {
        _observabilityService = observabilityService;
    }

    public async Task<IEnumerable<CheckResult>> Handle(GetLatencyTrendsQuery request, CancellationToken cancellationToken)
    {
        return await _observabilityService.GetLatencyTrendsAsync(request.EndpointId, request.Days);
    }
}