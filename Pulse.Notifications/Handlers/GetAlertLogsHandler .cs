
using MediatR;
using Pulse.Notifications.Entities;
using Pulse.Notifications.Interfaces;
using Pulse.Notifications.Queries;

namespace Pulse.Notifications.Handlers;

public class GetAlertLogsHandler : IRequestHandler<GetAlertLogsQuery, IEnumerable<AlertLog>>
{
    private readonly INotificationService _notificationService;

    public GetAlertLogsHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<AlertLog>> Handle(GetAlertLogsQuery request, CancellationToken cancellationToken)
    {
        return await _notificationService.GetAlertLogsAsync(request.EndpointId);
    }
}