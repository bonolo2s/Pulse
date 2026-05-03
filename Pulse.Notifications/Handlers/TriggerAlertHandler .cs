using MediatR;
using Pulse.Notifications.Commands;
using Pulse.Notifications.Interfaces;

namespace Pulse.Notifications.Handlers;

public class TriggerAlertHandler : IRequestHandler<TriggerAlertCommand>
{
    private readonly INotificationService _notificationService;

    public TriggerAlertHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(TriggerAlertCommand request, CancellationToken cancellationToken)
    {
        await _notificationService.TriggerAlertAsync(request.Result);
    }
}