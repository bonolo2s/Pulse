namespace Pulse.Notifications.Entities;

public class AlertRule
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public AlertChannel Channel { get; set; } // Email | Slack | Sms
    public string Destination { get; set; } = string.Empty; // address of chanell of choice
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}