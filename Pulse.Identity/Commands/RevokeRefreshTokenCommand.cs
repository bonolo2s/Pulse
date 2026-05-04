using MediatR;

namespace Pulse.Identity.Commands;

public record RevokeRefreshTokenCommand(Guid UserId, string RefreshToken) : IRequest;