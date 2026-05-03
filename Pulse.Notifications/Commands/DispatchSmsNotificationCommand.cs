using MediatR;
using Pulse.Notifications.Entities;

namespace Pulse.Notifications.Commands;

public record DispatchSmsNotificationCommand(AlertLog Log) : IRequest;