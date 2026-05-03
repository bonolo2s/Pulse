using MediatR;

namespace Pulse.StatusPages.Commands;

public record UpdateStatusPageCommand(Guid Id, StatusPage StatusPage) : IRequest<StatusPage>;