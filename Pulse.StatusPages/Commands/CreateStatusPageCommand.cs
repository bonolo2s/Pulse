using MediatR;

namespace Pulse.StatusPages.Commands;

public record CreateStatusPageCommand(StatusPage StatusPage) : IRequest<StatusPage>;