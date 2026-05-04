using MediatR;
namespace Pulse.Identity.Commands;

public record UpdateUserCommand(Guid UserId, User Updated) : IRequest<User>;