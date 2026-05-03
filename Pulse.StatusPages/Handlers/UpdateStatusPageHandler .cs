using MediatR;
using Pulse.StatusPages.Commands;
using Pulse.StatusPages.Interfaces;

namespace Pulse.StatusPages.Handlers;

public class UpdateStatusPageHandler : IRequestHandler<UpdateStatusPageCommand, StatusPage>
{
    private readonly IStatusPageService _statusPageService;

    public UpdateStatusPageHandler(IStatusPageService statusPageService)
    {
        _statusPageService = statusPageService;
    }

    public async Task<StatusPage> Handle(UpdateStatusPageCommand request, CancellationToken cancellationToken)
    {
        return await _statusPageService.UpdateStatusPageAsync(request.Id, request.StatusPage);
    }
}