using MediatR;
using Pulse.StatusPages.Commands;
using Pulse.StatusPages.Interfaces;

namespace Pulse.StatusPages.Handlers;

public class AddEndpointToStatusPageHandler : IRequestHandler<AddEndpointToStatusPageCommand, StatusPageEndpoint>
{
    private readonly IStatusPageService _statusPageService;

    public AddEndpointToStatusPageHandler(IStatusPageService statusPageService)
    {
        _statusPageService = statusPageService;
    }

    public async Task<StatusPageEndpoint> Handle(AddEndpointToStatusPageCommand request, CancellationToken cancellationToken)
    {
        return await _statusPageService.AddEndpointToStatusPageAsync(request.StatusPageEndpoint);
    }
}