using MediatR;
using Pulse.Notifications.Entities;
using Pulse.Notifications.Interfaces;
using Pulse.Notifications.Queries;

namespace Pulse.Notifications.Handlers;

public class GetAlertSettingsHandler : IRequestHandler<GetAlertSettingsQuery, IEnumerable<AlertRule>>
{
    private readonly INotificationService _notificationService;

    public GetAlertSettingsHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<AlertRule>> Handle(GetAlertSettingsQuery request, CancellationToken cancellationToken)
    {
        return await _notificationService.GetAlertSettingsAsync(request.UserId);
    }
}