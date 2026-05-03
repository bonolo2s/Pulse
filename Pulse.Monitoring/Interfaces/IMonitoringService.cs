namespace Pulse.Monitoring.Interfaces;

public interface IMonitoringService
{
    Task<MonitoredEndpoint> AddEndpointAsync(MonitoredEndpoint endpoint);
    Task<MonitoredEndpoint> EditEndpointAsync(Guid id, MonitoredEndpoint endpoint);
    Task RemoveEndpointAsync(Guid id);
    Task ToggleMonitorStatusAsync(Guid id);
    Task<IEnumerable<MonitoredEndpoint>> GetEndpointsAsync(Guid userId);
    Task<int> GetEndpointCountAsync(Guid userId);
}