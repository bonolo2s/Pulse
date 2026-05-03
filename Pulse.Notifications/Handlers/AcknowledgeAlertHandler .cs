using MediatR;
using Pulse.Notifications.Commands;
using Pulse.Notifications.Interfaces;

namespace Pulse.Notifications.Handlers;

public class AcknowledgeAlertHandler : IRequestHandler<AcknowledgeAlertCommand>
{
    private readonly INotificationService _notificationService;

    public AcknowledgeAlertHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(AcknowledgeAlertCommand request, CancellationToken cancellationToken)
    {
        await _notificationService.AcknowledgeAlertAsync(request.AlertLogId);
    }
}