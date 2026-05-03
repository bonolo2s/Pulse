using MediatR;

namespace Pulse.StatusPages.Commands;

public record AddEndpointToStatusPageCommand(StatusPageEndpoint StatusPageEndpoint) : IRequest<StatusPageEndpoint>;