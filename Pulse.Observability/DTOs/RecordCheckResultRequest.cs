using Pulse.Observability.Entities;

namespace Pulse.Observability.DTOs;

public class RecordCheckResultRequest
{
    public Guid EndpointId { get; set; }
    public EndpointStatus Status { get; set; }
    public int StatusCode { get; set; }
    public long LatencyMs { get; set; }
    public string? SslIssuer { get; set; }
    public DateTime? SslExpiresAt { get; set; }
    public int? SslDaysRemaining { get; set; }
    public string? ErrorMessage { get; set; }
}