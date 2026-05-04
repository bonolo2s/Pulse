namespace Pulse.Identity.Interfaces;

public interface IAuthService
{
    Task<User> RegisterUserAsync(User user, string password);
    Task<string> LoginUserAsync(string email, string password);
    Task<User> GetCurrentUserAsync(Guid userId);
    Task<User> UpdateUserAsync(Guid userId, User updated);
    Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task DeleteAccountAsync(Guid userId);
}