namespace Pulse.Monitoring.DTOs;

public class AddEndpointRequest
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public int IntervalSeconds { get; set; }
    public int TimeoutMs { get; set; }
}