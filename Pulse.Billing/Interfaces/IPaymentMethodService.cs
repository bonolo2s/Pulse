using Pulse.Billing.Entities;

namespace Pulse.Billing.Interfaces;

public interface IPaymentMethodService
{
    Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync(Guid userId);
    Task SavePaymentMethodAsync(PaymentMethod paymentMethod);
    Task DeletePaymentMethodAsync(Guid id);
    Task SetDefaultPaymentMethodAsync(Guid id);
}