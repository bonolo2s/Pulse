using Pulse.Notifications.Entities;
using Pulse.Shared.DTOs;

namespace Pulse.Notifications.Interfaces;

public interface INotificationService
{
    Task TriggerAlertAsync(HealthCheckResult result);
    Task DispatchEmailNotificationAsync(AlertLog log);
    Task DispatchSmsNotificationAsync(AlertLog log);
    Task DispatchSlackNotificationAsync(AlertLog log);
    Task ResolveAlertAsync(Guid endpointId);
    Task ManageAlertRulesAsync(AlertRule rule);
    Task<IEnumerable<AlertRule>> GetAlertSettingsAsync(Guid userId);
    Task<IEnumerable<AlertLog>> GetAlertLogsAsync(Guid endpointId);
    Task AcknowledgeAlertAsync(Guid alertLogId);
}