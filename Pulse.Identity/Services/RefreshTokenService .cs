using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pulse.Identity.DataAccess;
using Pulse.Identity.Interfaces;
using System.Security.Cryptography;

namespace Pulse.Identity.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IdentityDbContext _context;
    private readonly IConfiguration _configuration;

    public RefreshTokenService(IdentityDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<RefreshToken> GenerateRefreshToken(User user)
    {
        var expiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7");

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<bool> ValidateRefreshToken(Guid userId, string token)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Token == token);

        if (refreshToken is null) return false;
        if (refreshToken.IsRevoked) return false;
        if (refreshToken.ExpiresAt < DateTime.UtcNow) return false;

        return true;
    }

    public async Task RevokeRefreshToken(Guid userId, string token)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Token == token);

        if (refreshToken is null || refreshToken.IsRevoked) return;

        refreshToken.IsRevoked = true;
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllUserTokens(Guid userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
            token.IsRevoked = true;

        await _context.SaveChangesAsync();
    }
}