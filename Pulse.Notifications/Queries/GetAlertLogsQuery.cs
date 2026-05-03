using MediatR;
using Pulse.Notifications.Entities;

namespace Pulse.Notifications.Queries;

public record GetAlertLogsQuery(Guid EndpointId) : IRequest<IEnumerable<AlertLog>>;