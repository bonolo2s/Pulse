using MediatR;
using Pulse.Billing.Payments.Paystack.DTOs;

namespace Pulse.Billing.Commands;

public record ProcessPaymentResultCommand(
    string PaymentReference,
    string Status,
    string? Channel,
    PaystackAuthorization? Authorization) : IRequest;