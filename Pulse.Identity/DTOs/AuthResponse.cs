namespace Pulse.Identity.DTOs;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public AuthUserResponse User { get; set; } = null!;
}

public class AuthUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Tier { get; set; } = string.Empty;
    public IEnumerable<string> Permissions { get; set; } = [];
}