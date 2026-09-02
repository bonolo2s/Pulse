using MediatR;

namespace Pulse.Billing.Commands;
public record ActivateSubscriptionCommand(string Email, string SubscriptionCode, string EmailToken, string CustomerCode) : IRequest;