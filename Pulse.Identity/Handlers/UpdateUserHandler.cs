using MediatR;
using Pulse.Identity.Commands;
using Pulse.Identity.Interfaces;

namespace Pulse.Identity.Handlers;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, User>
{
    private readonly IAuthService _authService;

    public UpdateUserHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await _authService.UpdateUserAsync(request.UserId, request.Updated);
    }
}