using MediatR;
using Pulse.Notifications.Commands;
using Pulse.Notifications.Interfaces;

namespace Pulse.Notifications.Handlers;

public class ResolveAlertHandler : IRequestHandler<ResolveAlertCommand>
{
    private readonly INotificationService _notificationService;

    public ResolveAlertHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(ResolveAlertCommand request, CancellationToken cancellationToken)
    {
        await _notificationService.ResolveAlertAsync(request.EndpointId);
    }
}