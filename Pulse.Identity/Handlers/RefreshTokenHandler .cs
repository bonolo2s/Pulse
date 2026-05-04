using MediatR;
using Pulse.Identity.Commands;
using Pulse.Identity.DataAccess;
using Pulse.Identity.DTOs;
using Pulse.Identity.Interfaces;

namespace Pulse.Identity.Handlers;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ITokenService _tokenService;
    private readonly IdentityDbContext _context;

    public RefreshTokenHandler(IRefreshTokenService refreshTokenService, ITokenService tokenService, IdentityDbContext context)
    {
        _refreshTokenService = refreshTokenService;
        _tokenService = tokenService;
        _context = context;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var isValid = await _refreshTokenService.ValidateRefreshToken(request.UserId, request.RefreshToken);
        if (!isValid) throw new UnauthorizedAccessException("Invalid or expired refresh token");

        var user = await _context.Users.FindAsync(request.UserId);
        if (user is null) throw new UnauthorizedAccessException("User not found");

        await _refreshTokenService.RevokeRefreshToken(request.UserId, request.RefreshToken);

        var newRefreshToken = await _refreshTokenService.GenerateRefreshToken(user);
        var authResponse = _tokenService.GenerateToken(user);
        authResponse.RefreshToken = newRefreshToken.Token;

        return authResponse;
    }
}