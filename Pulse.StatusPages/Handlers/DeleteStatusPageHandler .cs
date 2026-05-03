using MediatR;
using Pulse.StatusPages.Commands;
using Pulse.StatusPages.Interfaces;

namespace Pulse.StatusPages.Handlers;

public class DeleteStatusPageHandler : IRequestHandler<DeleteStatusPageCommand>
{
    private readonly IStatusPageService _statusPageService;

    public DeleteStatusPageHandler(IStatusPageService statusPageService)
    {
        _statusPageService = statusPageService;
    }

    public async Task Handle(DeleteStatusPageCommand request, CancellationToken cancellationToken)
    {
        await _statusPageService.DeleteStatusPageAsync(request.Id);
    }
}