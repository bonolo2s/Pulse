using MediatR;
using Pulse.Identity.DTOs;

namespace Pulse.Identity.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<AuthResponse>;