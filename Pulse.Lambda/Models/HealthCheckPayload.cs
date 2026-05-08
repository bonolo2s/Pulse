namespace Pulse.Lambda.Models;

public class HealthCheckPayload
{
    public Guid EndpointId { get; init; }
    public string Url { get; init; } = string.Empty;
    public int CheckIntervalSeconds { get; init; }
}