using MediatR;
using Pulse.Observability.Entities;

namespace Pulse.Observability.Queries;

public record GetSslExpiryStatusQuery(Guid EndpointId) : IRequest<CheckResult?>;