using Pulse.Notifications.Entities;

namespace Pulse.Notifications.DTOs;

public class ManageAlertRulesRequest
{
    public Guid AlertRuleId { get; set; }
    public Guid UserId { get; set; }
    public AlertChannel Channel { get; set; }
    public string Destination { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}