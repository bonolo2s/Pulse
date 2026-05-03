using Pulse.Notifications.Entities;

namespace Pulse.Notifications.DTOs;

public class AlertLogResponse
{
    public Guid Id { get; set; }
    public Guid AlertRuleId { get; set; }
    public Guid EndpointId { get; set; }
    public AlertChannel Channel { get; set; }
    public AlertType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Delivered { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public DateTime SentAt { get; set; }
}