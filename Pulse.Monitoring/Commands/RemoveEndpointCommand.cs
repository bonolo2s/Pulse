using MediatR;

namespace Pulse.Monitoring.Commands;

public record RemoveEndpointCommand(Guid Id) : IRequest;