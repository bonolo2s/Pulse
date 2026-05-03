using MediatR;
using Pulse.Notifications.Commands;
using Pulse.Notifications.Interfaces;

namespace Pulse.Notifications.Handlers;

public class DispatchSmsNotificationHandler : IRequestHandler<DispatchSmsNotificationCommand>
{
    private readonly INotificationService _notificationService;

    public DispatchSmsNotificationHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(DispatchSmsNotificationCommand request, CancellationToken cancellationToken)
    {
        await _notificationService.DispatchSmsNotificationAsync(request.Log);
    }
}