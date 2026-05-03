public class AlertRule
{
    public Guid Id { get; set; }
    public Guid EndpointId { get; set; }
    public string Channel { get; set; } // Email | Slack | Webhook
    public string Destination { get; set; }
    public string Trigger { get; set; } // Down | Degraded | SslExpiry | Latency | Recovery
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}