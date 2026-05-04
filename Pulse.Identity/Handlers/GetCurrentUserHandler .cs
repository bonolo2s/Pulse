using MediatR;
using Pulse.Identity.Interfaces;
using Pulse.Identity.Queries;

namespace Pulse.Identity.Handlers;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, User>
{
    private readonly IAuthService _authService;

    public GetCurrentUserHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<User> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        return await _authService.GetCurrentUserAsync(request.UserId);
    }
}