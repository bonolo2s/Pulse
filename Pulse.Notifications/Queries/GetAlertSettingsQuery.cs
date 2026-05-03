using MediatR;
using Pulse.Notifications.Entities;

namespace Pulse.Notifications.Queries;

public record GetAlertSettingsQuery(Guid UserId) : IRequest<IEnumerable<AlertRule>>;