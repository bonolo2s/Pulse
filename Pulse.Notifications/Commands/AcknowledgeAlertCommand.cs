using MediatR;

namespace Pulse.Notifications.Commands;

public record AcknowledgeAlertCommand(Guid AlertLogId) : IRequest;