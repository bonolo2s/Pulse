using MediatR;

namespace Pulse.Notifications.Commands;

public record ResolveAlertCommand(Guid EndpointId) : IRequest;