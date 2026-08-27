using MediatR;
namespace Pulse.Billing.Commands;
public record RenewSubscriptionCommand(Guid SubscriptionId, string Email) : IRequest;