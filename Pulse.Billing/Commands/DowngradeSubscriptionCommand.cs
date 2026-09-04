using MediatR;

namespace Pulse.Billing.Commands;

public record DowngradeSubscriptionCommand(string SubscriptionCode) : IRequest;