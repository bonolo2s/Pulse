using MediatR;
using Pulse.Observability.Entities;
using Pulse.Observability.Interfaces;
using Pulse.Observability.Queries;

namespace Pulse.Observability.Handlers;

public class GetSslExpiryStatusHandler : IRequestHandler<GetSslExpiryStatusQuery, CheckResult?>
{
    private readonly IObservabilityService _observabilityService;

    public GetSslExpiryStatusHandler(IObservabilityService observabilityService)
    {
        _observabilityService = observabilityService;
    }

    public async Task<CheckResult?> Handle(GetSslExpiryStatusQuery request, CancellationToken cancellationToken)
    {
        return await _observabilityService.GetSslExpiryStatusAsync(request.EndpointId);
    }
}