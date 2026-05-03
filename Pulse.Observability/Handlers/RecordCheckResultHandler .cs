using MediatR;
using Pulse.Observability.Commands;
using Pulse.Observability.Interfaces;

namespace Pulse.Observability.Handlers;

public class RecordCheckResultHandler : IRequestHandler<RecordCheckResultCommand>
{
    private readonly IObservabilityService _observabilityService;

    public RecordCheckResultHandler(IObservabilityService observabilityService)
    {
        _observabilityService = observabilityService;
    }

    public async Task Handle(RecordCheckResultCommand request, CancellationToken cancellationToken)
    {
        await _observabilityService.RecordCheckResultAsync(request.Result);
    }
}