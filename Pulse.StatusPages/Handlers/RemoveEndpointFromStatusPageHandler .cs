using MediatR;
using Pulse.StatusPages.Commands;
using Pulse.StatusPages.Interfaces;

namespace Pulse.StatusPages.Handlers;

public class RemoveEndpointFromStatusPageHandler : IRequestHandler<RemoveEndpointFromStatusPageCommand>
{
    private readonly IStatusPageService _statusPageService;

    public RemoveEndpointFromStatusPageHandler(IStatusPageService statusPageService)
    {
        _statusPageService = statusPageService;
    }

    public async Task Handle(RemoveEndpointFromStatusPageCommand request, CancellationToken cancellationToken)
    {
        await _statusPageService.RemoveEndpointFromStatusPageAsync(request.StatusPageEndpointId);
    }
}