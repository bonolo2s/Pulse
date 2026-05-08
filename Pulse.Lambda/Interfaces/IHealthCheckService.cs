namespace Pulse.Lambda.Interfaces;

public interface IHealthCheckService
{
    Task<bool> PingAsync(string url);
    Task<long> MeasureLatencyAsync(string url);
    Task InspectSslAsync(string url);
    Task ReportResultAsync(Guid endpointId, bool isUp, long latencyMs);
}