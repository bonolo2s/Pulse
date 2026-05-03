using MediatR;
using Pulse.Notifications.Commands;
using Pulse.Notifications.Interfaces;

namespace Pulse.Notifications.Handlers;

public class DispatchSlackNotificationHandler : IRequestHandler<DispatchSlackNotificationCommand>
{
    private readonly INotificationService _notificationService;

    public DispatchSlackNotificationHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(DispatchSlackNotificationCommand request, CancellationToken cancellationToken)
    {
        await _notificationService.DispatchSlackNotificationAsync(request.Log);
    }
}