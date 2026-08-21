using Pulse.Billing.Entities;
using Pulse.Billing.Payments.Paystack.DTOs;
using System.Reflection.Metadata;
namespace Pulse.Billing.Interfaces;
public interface IBillingService
{
    Task ProcessPaymentResultAsync(string paymentReference, string status, string? channel, PaystackAuthorization? authorization);
    Task<Payment> CreatePendingPaymentAsync(Guid userId, Guid invoiceId, decimal amount, string providerReference);
}