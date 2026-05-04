using MediatR;

namespace Pulse.Identity.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<string>;