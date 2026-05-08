using Pulse.Shared.DTOs;

namespace Pulse.Lambda.Interfaces;

public interface IHealthCheckService
{
    Task<HealthCheckResult> RunHealthCheckAsync(Guid endpointId, string url);
}