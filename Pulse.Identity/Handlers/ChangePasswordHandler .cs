using MediatR;
using Pulse.Identity.Commands;
using Pulse.Identity.Interfaces;

namespace Pulse.Identity.Handlers;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IAuthService _authService;

    public ChangePasswordHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        await _authService.ChangePasswordAsync(request.UserId, request.CurrentPassword, request.NewPassword);
    }
}