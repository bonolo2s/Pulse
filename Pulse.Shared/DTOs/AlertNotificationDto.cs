namespace Pulse.Shared.DTOs;

public class AlertNotificationDto
{
    public Guid EndpointId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}