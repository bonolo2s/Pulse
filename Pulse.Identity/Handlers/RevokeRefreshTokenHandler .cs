using MediatR;
using Pulse.Identity.Commands;
using Pulse.Identity.Interfaces;

namespace Pulse.Identity.Handlers;

public class RevokeRefreshTokenHandler : IRequestHandler<RevokeRefreshTokenCommand>
{
    private readonly IRefreshTokenService _refreshTokenService;

    public RevokeRefreshTokenHandler(IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService = refreshTokenService;
    }

    public async Task Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        await _refreshTokenService.RevokeRefreshToken(request.UserId, request.RefreshToken);
    }
}