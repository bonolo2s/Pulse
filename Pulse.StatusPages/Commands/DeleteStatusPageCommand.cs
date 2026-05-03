using MediatR;

namespace Pulse.StatusPages.Commands;

public record DeleteStatusPageCommand(Guid Id) : IRequest;