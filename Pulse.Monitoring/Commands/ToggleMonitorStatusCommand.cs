using MediatR;

namespace Pulse.Monitoring.Commands;

public record ToggleMonitorStatusCommand(Guid Id) : IRequest;