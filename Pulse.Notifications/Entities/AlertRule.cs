namespace Pulse.Notifications.Entities;

public class AlertRule
{
    public Guid Id { get; set; }
    public Guid EndpointId { get; set; }
    public AlertChannel Channel { get; set; } // Email | Slack | Webhook
    public string Destination { get; set; } = string.Empty;
    public AlertTrigger Trigger { get; set; } // Down | Degraded | SslExpiry | Latency | Recovery
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}