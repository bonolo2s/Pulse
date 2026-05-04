using MediatR;
using Pulse.Identity.DTOs;

namespace Pulse.Identity.Commands;

public record RefreshTokenCommand(Guid UserId, string RefreshToken) : IRequest<AuthResponse>;