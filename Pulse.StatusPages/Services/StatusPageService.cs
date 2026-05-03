using Microsoft.EntityFrameworkCore;
using Pulse.StatusPages.DataAccess;
using Pulse.StatusPages.Interfaces;

namespace Pulse.StatusPages.Services;

public class StatusPageService : IStatusPageService
{
    private readonly StatusPagesDbContext _context;

    public StatusPageService(StatusPagesDbContext context)
    {
        _context = context;
    }

    public async Task<StatusPage> CreateStatusPageAsync(StatusPage statusPage)
    {
        statusPage.Id = Guid.NewGuid();
        statusPage.Slug = GenerateSlug(statusPage.Name);
        statusPage.CreatedAt = DateTime.UtcNow;

        _context.StatusPages.Add(statusPage);
        await _context.SaveChangesAsync();

        return statusPage;
    }

    public async Task<StatusPage> UpdateStatusPageAsync(Guid id, StatusPage updated)
    {
        var statusPage = await _context.StatusPages.FindAsync(id)
            ?? throw new KeyNotFoundException($"Status page {id} not found.");

        statusPage.Name = updated.Name;
        statusPage.Slug = GenerateSlug(updated.Name);
        statusPage.IsPublic = updated.IsPublic;

        await _context.SaveChangesAsync();

        return statusPage;
    }

    public async Task DeleteStatusPageAsync(Guid id)
    {
        var statusPage = await _context.StatusPages.FindAsync(id)
            ?? throw new KeyNotFoundException($"Status page {id} not found.");

        _context.StatusPages.Remove(statusPage);
        await _context.SaveChangesAsync();
    }

    public async Task<StatusPageEndpoint> AddEndpointToStatusPageAsync(StatusPageEndpoint statusPageEndpoint)
    {
        statusPageEndpoint.Id = Guid.NewGuid();

        _context.StatusPageEndpoints.Add(statusPageEndpoint);
        await _context.SaveChangesAsync();

        return statusPageEndpoint;
    }

    public async Task RemoveEndpointFromStatusPageAsync(Guid statusPageEndpointId)
    {
        var statusPageEndpoint = await _context.StatusPageEndpoints.FindAsync(statusPageEndpointId)
            ?? throw new KeyNotFoundException($"Status page endpoint {statusPageEndpointId} not found.");

        _context.StatusPageEndpoints.Remove(statusPageEndpoint);
        await _context.SaveChangesAsync();
    }

    public async Task<StatusPage> GetPublicStatusPageAsync(string slug)
    {
        return await _context.StatusPages
            .Include(s => s.Endpoints)
            .Where(s => s.Slug == slug && s.IsPublic)
            .FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException($"Public status page '{slug}' not found.");
    }

    public async Task<StatusPage> GetPrivateStatusPageAsync(Guid statusPageId)
    {
        return await _context.StatusPages
            .Include(s => s.Endpoints)
            .Where(s => s.Id == statusPageId)
            .FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException($"Status page {statusPageId} not found.");
    }

    private static string GenerateSlug(string name)
    {
        return name.ToLower().Replace(" ", "-");
    }
}