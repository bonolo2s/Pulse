using MediatR;
using Pulse.Notifications.Entities;

namespace Pulse.Notifications.Commands;

public record DispatchEmailNotificationCommand(AlertLog Log) : IRequest;