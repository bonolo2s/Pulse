using MediatR;
using Pulse.Shared.DTOs;

namespace Pulse.Notifications.Commands;

public record TriggerAlertCommand(HealthCheckResult Result) : IRequest;