namespace Pulse.Identity.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshToken(User user);
    Task<bool> ValidateRefreshToken(Guid userId, string token);
    Task RevokeRefreshToken(Guid userId, string token);
    Task RevokeAllUserTokens(Guid userId);
}