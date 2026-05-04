using Microsoft.EntityFrameworkCore;
using Pulse.Identity.DataAccess;
using Pulse.Identity.Interfaces;
using Pulse.Shared.Interfaces;

namespace Pulse.Identity.Services;

public class AuthService : IAuthService
{
    private readonly IdentityDbContext _context;
    private readonly ISubscriptionCreator _subscriptionCreator;

    public AuthService(IdentityDbContext context, ISubscriptionCreator subscriptionCreator)
    {
        _context = context;
        _subscriptionCreator = subscriptionCreator;
    }

    public async Task<User> RegisterUserAsync(User user, string password)
    {
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == user.Email);

        if (emailExists)
            throw new InvalidOperationException($"Email {user.Email} is already registered.");

        user.Id = Guid.NewGuid();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        user.Plan = "Free";
        user.CreatedAt = DateTime.UtcNow;
        user.IsActive = true;

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _subscriptionCreator.CreateSubscriptionAsync(user.Id);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return user;
    }

    public async Task<string> LoginUserAsync(string email, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        var passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

        if (!passwordValid)
            throw new UnauthorizedAccessException("Invalid email or password.");

        // dont fortet the JWT boy
        return $"JWT_TOKEN_PLACEHOLDER_{user.Id}";
    }

    public async Task<User> GetCurrentUserAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException($"User {userId} not found.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is deactivated.");

        return user;
    }

    public async Task<User> UpdateUserAsync(Guid userId, User updated)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException($"User {userId} not found.");

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == updated.Email && u.Id != userId);

        if (emailExists)
            throw new InvalidOperationException($"Email {updated.Email} is already in use.");

        user.FullName = updated.FullName;
        user.Email = updated.Email;

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException($"User {userId} not found.");

        var passwordValid = BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash);

        if (!passwordValid)
            throw new UnauthorizedAccessException("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAccountAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException($"User {userId} not found.");

        user.IsActive = false;

        await _context.SaveChangesAsync();
    }
}