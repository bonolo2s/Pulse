public class CheckResult
{
    public Guid Id { get; set; }
    public Guid EndpointId { get; set; }
    public string Status { get; set; } // Operational | Degraded | Downtime
    public int StatusCode { get; set; }
    public long LatencyMs { get; set; }
    public string? SslIssuer { get; set; }
    public DateTime? SslExpiresAt { get; set; }
    public int? SslDaysRemaining { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CheckedAt { get; set; }
}