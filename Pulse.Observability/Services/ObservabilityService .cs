using Microsoft.EntityFrameworkCore;
using Pulse.Observability.DataAccess;
using Pulse.Observability.Entities;
using Pulse.Observability.Interfaces;
using Pulse.Shared.Enums;

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

    public async Task<CheckResult?> GetLatestCheckResultAsync(Guid endpointId)
    {
        return await _context.CheckResults
            .Where(c => c.EndpointId == endpointId)
            .OrderByDescending(c => c.CheckedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<double> GetUptimePercentageAsync(Guid endpointId, int days = 30)
    {
        var history = await GetUptimeHistoryAsync(endpointId, days);
        var results = history.ToList();

        if (results.Count == 0)
            return 0;

        var upCount = results.Count(r => r.Status == EndpointStatus.Operational);
        return Math.Round((upCount / (double)results.Count) * 100, 2);
    }

    public async Task<Dictionary<Guid, CheckResult>> GetLatestCheckResultsAsync(IEnumerable<Guid> endpointIds)
    {
        var ids = endpointIds.ToList();

        var results = await _context.CheckResults
            .Where(c => ids.Contains(c.EndpointId))
            .GroupBy(c => c.EndpointId)
            .Select(g => g.OrderByDescending(c => c.CheckedAt).First())
            .ToListAsync();

        return results.ToDictionary(r => r.EndpointId, r => r);
    }

    public async Task<Dictionary<Guid, double>> GetUptimePercentagesAsync(IEnumerable<Guid> endpointIds, int days = 30)
    {
        var ids = endpointIds.ToList();
        var from = DateTime.UtcNow.AddDays(-days);

        var history = await _context.CheckResults
            .Where(c => ids.Contains(c.EndpointId) && c.CheckedAt >= from)
            .ToListAsync();

        return ids.ToDictionary(
            id => id,
            id =>
            {
                var group = history.Where(c => c.EndpointId == id).ToList();
                if (group.Count == 0) return 0;
                var upCount = group.Count(r => r.Status == EndpointStatus.Operational);
                return Math.Round((upCount / (double)group.Count) * 100, 2);
            });
    }
}