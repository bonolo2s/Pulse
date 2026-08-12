using MediatR;

namespace Pulse.Shared.Events;

public record UserRegisteredNotification(Guid UserId, string Email, string FullName) : INotification;