namespace Pulse.Identity.DTOs;

public class RefreshTokenRequest
{
    public Guid UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}