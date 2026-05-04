using MediatR;
using Pulse.Identity.Commands;
using Pulse.Identity.DTOs;
using Pulse.Identity.Interfaces;

namespace Pulse.Identity.Handlers;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, AuthResponse>
{
    private readonly IAuthService _authService;

    public LoginUserHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        return await _authService.LoginUserAsync(request.Email, request.Password);
    }
}