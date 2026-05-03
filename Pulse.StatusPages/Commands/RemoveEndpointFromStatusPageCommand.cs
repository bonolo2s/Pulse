using MediatR;

namespace Pulse.StatusPages.Commands;

public record RemoveEndpointFromStatusPageCommand(Guid StatusPageEndpointId) : IRequest;