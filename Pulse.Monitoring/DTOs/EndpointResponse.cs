namespace Pulse.Monitoring.DTOs;

public class EndpointResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public int IntervalSeconds { get; set; }
    public int TimeoutMs { get; set; }
    public string? Status { get; set; }
    public long? LatencyMs { get; set; }
    public DateTime? LastCheckedAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}