using MediatR;

namespace Pulse.StatusPages.Queries;

public record GetPublicStatusPageQuery(string Slug) : IRequest<StatusPage>;