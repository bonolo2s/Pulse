using Pulse.Identity.DTOs;

namespace Pulse.Identity.Interfaces
{
    public interface ITokenService
    {
        AuthResponse GenerateToken(User user);
    }
}
