using MediatR;
using Pulse.StatusPages.Commands;
using Pulse.StatusPages.Interfaces;

namespace Pulse.StatusPages.Handlers;

public class CreateStatusPageHandler : IRequestHandler<CreateStatusPageCommand, StatusPage>
{
    private readonly IStatusPageService _statusPageService;

    public CreateStatusPageHandler(IStatusPageService statusPageService)
    {
        _statusPageService = statusPageService;
    }

    public async Task<StatusPage> Handle(CreateStatusPageCommand request, CancellationToken cancellationToken)
    {
        return await _statusPageService.CreateStatusPageAsync(request.StatusPage);
    }
}