using MediatR;
using Pulse.Notifications.Entities;

namespace Pulse.Notifications.Commands;

public record DispatchSlackNotificationCommand(AlertLog Log) : IRequest;