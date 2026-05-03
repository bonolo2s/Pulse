using MediatR;
using Pulse.StatusPages.Interfaces;
using Pulse.StatusPages.Queries;

namespace Pulse.StatusPages.Handlers;

public class GetPrivateStatusPageHandler : IRequestHandler<GetPrivateStatusPageQuery, StatusPage>
{
    private readonly IStatusPageService _statusPageService;

    public GetPrivateStatusPageHandler(IStatusPageService statusPageService)
    {
        _statusPageService = statusPageService;
    }

    public async Task<StatusPage> Handle(GetPrivateStatusPageQuery request, CancellationToken cancellationToken)
    {
        return await _statusPageService.GetPrivateStatusPageAsync(request.StatusPageId);
    }
}