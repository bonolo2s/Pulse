namespace Pulse.Identity.DTOs;

public class RevokeRefreshTokenRequest
{
    public Guid UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}