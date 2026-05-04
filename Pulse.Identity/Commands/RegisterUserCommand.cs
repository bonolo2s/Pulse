using MediatR;
namespace Pulse.Identity.Commands;

public record RegisterUserCommand(User User, string Password) : IRequest<User>;