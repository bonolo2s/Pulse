using Pulse.Shared.DTOs;

namespace Pulse.Shared.Interfaces;

public interface ISnsPublisher
{
    Task PublishAlertAsync(AlertNotificationDto message);
}