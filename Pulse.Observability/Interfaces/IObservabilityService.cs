using Pulse.Observability.Entities;

namespace Pulse.Observability.Interfaces;

public interface IObservabilityService
{
    Task RecordCheckResultAsync(CheckResult result);
    Task<IEnumerable<CheckResult>> GetUptimeHistoryAsync(Guid endpointId, int days);
    Task<IEnumerable<CheckResult>> GetLatencyTrendsAsync(Guid endpointId, int days);
    Task<CheckResult?> GetSslExpiryStatusAsync(Guid endpointId);
    Task<CheckResult?> GetLatestCheckResultAsync(Guid endpointId);
    Task<double> GetUptimePercentageAsync(Guid endpointId, int days = 30);
}