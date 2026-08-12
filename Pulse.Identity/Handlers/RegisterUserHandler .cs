using MediatR;
using Pulse.Identity.Commands;
using Pulse.Identity.Interfaces;
using Pulse.Shared.Events;

namespace Pulse.Identity.Handlers;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, User>
{
    private readonly IAuthService _authService;
    private readonly IMediator _mediator;

    public RegisterUserHandler(IAuthService authService, IMediator mediator)
    {
        _authService = authService;
        _mediator = mediator;
    }

    public async Task<User> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _authService.RegisterUserAsync(request.User, request.Password);

        await _mediator.Publish(new UserRegisteredNotification(user.Id, user.Email, user.FullName), cancellationToken);

        return user;
    }
}