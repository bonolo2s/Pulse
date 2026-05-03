using Microsoft.EntityFrameworkCore;
using Pulse.Observability.DataAccess;
using Pulse.Observability.Entities;
using Pulse.Observability.Interfaces;

namespace Pulse.Observability.Services;

public class ObservabilityService : IObservabilityService
{
    private readonly ObservabilityDbContext _context;

    public ObservabilityService(ObservabilityDbContext context)
    {
        _context = context;
    }

    public async Task RecordCheckResultAsync(CheckResult result)
    {
        result.Id = Guid.NewGuid();
        result.CheckedAt = DateTime.UtcNow;

        _context.CheckResults.Add(result);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CheckResult>> GetUptimeHistoryAsync(Guid endpointId, int days)
    {
        var from = DateTime.UtcNow.AddDays(-days);

        return await _context.CheckResults
            .Where(c => c.EndpointId == endpointId && c.CheckedAt >= from)
            .OrderByDescending(c => c.CheckedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CheckResult>> GetLatencyTrendsAsync(Guid endpointId, int days)
    {
        var from = DateTime.UtcNow.AddDays(-days);

        return await _context.CheckResults
            .Where(c => c.EndpointId == endpointId && c.CheckedAt >= from)
            .OrderBy(c => c.CheckedAt)
            .ToListAsync();
    }

    public async Task<CheckResult?> GetSslExpiryStatusAsync(Guid endpointId)
    {
        return await _context.CheckResults
            .Where(c => c.EndpointId == endpointId && c.SslExpiresAt != null)
            .OrderByDescending(c => c.CheckedAt)
            .FirstOrDefaultAsync();
    }
}