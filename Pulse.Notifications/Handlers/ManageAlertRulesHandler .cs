using MediatR;
using Pulse.Notifications.Commands;
using Pulse.Notifications.Interfaces;

namespace Pulse.Notifications.Handlers;

public class ManageAlertRulesHandler : IRequestHandler<ManageAlertRulesCommand>
{
    private readonly INotificationService _notificationService;

    public ManageAlertRulesHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(ManageAlertRulesCommand request, CancellationToken cancellationToken)
    {
        await _notificationService.ManageAlertRulesAsync(request.Rule);
    }
}