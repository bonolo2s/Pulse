using Pulse.Billing.Entities;
using System.Reflection.Metadata;

namespace Pulse.Billing.Interfaces;

public interface IBillingService
{
    Task ProcessPaymentResultAsync(string paymentReference, string status);
    Task<Payment> CreatePendingPaymentAsync(Guid userId, Guid invoiceId, decimal amount, string providerReference);
}