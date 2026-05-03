public class AlertLog
{
    public Guid Id { get; set; }
    public Guid AlertRuleId { get; set; }
    public Guid EndpointId { get; set; }
    public string Channel { get; set; }
    public string Type { get; set; } // Downtime | Degraded | Recovery | Latency | SslExpiry
    public string Message { get; set; }
    public bool Delivered { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public DateTime SentAt { get; set; }
}