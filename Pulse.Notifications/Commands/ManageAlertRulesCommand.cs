using MediatR;
using Pulse.Notifications.Entities;

namespace Pulse.Notifications.Commands;

public record ManageAlertRulesCommand(AlertRule Rule) : IRequest;