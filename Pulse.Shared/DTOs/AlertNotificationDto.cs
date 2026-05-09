namespace Pulse.Shared.DTOs;

public class AlertNotificationDto
{
    public Guid EndpointId { get; set; }
    public HealthCheckResult Result { get; set; } = null!;
    public DateTime SentAt { get; set; }
}