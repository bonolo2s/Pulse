using MediatR;
using Pulse.Notifications.Commands;
using Pulse.Notifications.Interfaces;

namespace Pulse.Notifications.Handlers;

public class DispatchEmailNotificationHandler : IRequestHandler<DispatchEmailNotificationCommand>
{
    private readonly INotificationService _notificationService;

    public DispatchEmailNotificationHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(DispatchEmailNotificationCommand request, CancellationToken cancellationToken)
    {
        await _notificationService.DispatchEmailNotificationAsync(request.Log);
    }
}