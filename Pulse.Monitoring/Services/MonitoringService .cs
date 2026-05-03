using Microsoft.EntityFrameworkCore;
using Pulse.Monitoring.DataAccess;
using Pulse.Monitoring.Interfaces;

namespace Pulse.Monitoring.Services;

public class MonitoringService : IMonitoringService
{
    private readonly MonitoringDbContext _context;

    public MonitoringService(MonitoringDbContext context)
    {
        _context = context;
    }

    public async Task<MonitoredEndpoint> AddEndpointAsync(MonitoredEndpoint endpoint)
    {
        endpoint.Id = Guid.NewGuid();
        endpoint.CreatedAt = DateTime.UtcNow;
        endpoint.IsActive = true;

        _context.MonitoredEndpoints.Add(endpoint);
        await _context.SaveChangesAsync();

        return endpoint;
    }

    public async Task<MonitoredEndpoint> EditEndpointAsync(Guid id, MonitoredEndpoint updated)
    {
        var endpoint = await _context.MonitoredEndpoints.FindAsync(id)
            ?? throw new KeyNotFoundException($"Endpoint {id} not found.");

        endpoint.Name = updated.Name;
        endpoint.Url = updated.Url;
        endpoint.Method = updated.Method;
        endpoint.IntervalSeconds = updated.IntervalSeconds;
        endpoint.TimeoutMs = updated.TimeoutMs;

        await _context.SaveChangesAsync();

        return endpoint;
    }

    public async Task RemoveEndpointAsync(Guid id)
    {
        var endpoint = await _context.MonitoredEndpoints.FindAsync(id)
            ?? throw new KeyNotFoundException($"Endpoint {id} not found.");

        _context.MonitoredEndpoints.Remove(endpoint);
        await _context.SaveChangesAsync();
    }

    public async Task ToggleMonitorStatusAsync(Guid id)
    {
        var endpoint = await _context.MonitoredEndpoints.FindAsync(id)
            ?? throw new KeyNotFoundException($"Endpoint {id} not found.");

        endpoint.IsActive = !endpoint.IsActive;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<MonitoredEndpoint>> GetEndpointsAsync(Guid userId)
    {
        return await _context.MonitoredEndpoints
            .Where(e => e.UserId == userId)
            .ToListAsync();
    }

    public async Task<int> GetEndpointCountAsync(Guid userId)
    {
        return await _context.MonitoredEndpoints
            .CountAsync(e => e.UserId == userId);
    }
}