using MediatR;
using Pulse.Billing.Payments.Paystack.DTOs;

namespace Pulse.Billing.Commands;

public record InitiateCheckoutCommand(Guid UserId, string Email) : IRequest<InitializeTransactionResult>;