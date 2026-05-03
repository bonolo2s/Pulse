using Pulse.Shared.Enums;

namespace Pulse.Observability.Entities;

public class CheckResult
{
    public Guid Id { get; set; }
    public Guid EndpointId { get; set; }
    public EndpointStatus Status { get; set; } // Operational | Degraded | Downtime
    public int StatusCode { get; set; } // HTTP response code e.g. 200, 404, 500
    public long LatencyMs { get; set; } 
    public string? SslIssuer { get; set; } // Certificate authority e.g. Let's Encrypt
    public DateTime? SslExpiresAt { get; set; } 
    public int? SslDaysRemaining { get; set; } 
    public string? ErrorMessage { get; set; } // Populates on failure or degradation
    public DateTime CheckedAt { get; set; } 
}