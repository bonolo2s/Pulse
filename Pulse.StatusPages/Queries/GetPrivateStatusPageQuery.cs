using MediatR;

namespace Pulse.StatusPages.Queries;

public record GetPrivateStatusPageQuery(Guid StatusPageId) : IRequest<StatusPage>;