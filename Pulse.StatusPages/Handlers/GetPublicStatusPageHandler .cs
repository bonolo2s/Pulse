using MediatR;
using Pulse.StatusPages.Interfaces;
using Pulse.StatusPages.Queries;

namespace Pulse.StatusPages.Handlers;

public class GetPublicStatusPageHandler : IRequestHandler<GetPublicStatusPageQuery, StatusPage>
{
    private readonly IStatusPageService _statusPageService;

    public GetPublicStatusPageHandler(IStatusPageService statusPageService)
    {
        _statusPageService = statusPageService;
    }

    public async Task<StatusPage> Handle(GetPublicStatusPageQuery request, CancellationToken cancellationToken)
    {
        return await _statusPageService.GetPublicStatusPageAsync(request.Slug);
    }
}