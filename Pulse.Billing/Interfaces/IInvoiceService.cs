using Pulse.Billing.Entities;
using Pulse.Billing.Enums;

namespace Pulse.Billing.Interfaces;

public interface IInvoiceService
{
    Task<IEnumerable<Invoice>> GetBillingHistoryAsync(Guid userId);
    //Task<Invoice> CreatePendingInvoiceAsync(Guid userId, Guid subscriptionId, decimal amount, string currency, InvoiceType type);
}