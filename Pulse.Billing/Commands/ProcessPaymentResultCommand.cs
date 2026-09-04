using MediatR;
using Pulse.Billing.Payments.Paystack.DTOs;

public record ProcessPaymentResultCommand(
    string PaymentReference,
    string EventId,
    string Status,
    string? Channel,
    string Email,
    PaystackAuthorization? Authorization) : IRequest;