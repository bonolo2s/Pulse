using MediatR;

namespace Pulse.Identity.Queries;

public record GetCurrentUserQuery(Guid UserId) : IRequest<User>;