using Microsoft.EntityFrameworkCore;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Entities;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Services;

public class PaymentMethodService : IPaymentMethodService
{
    private readonly BillingDbContext _context;

    public PaymentMethodService(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync(Guid userId)
    {
        return await _context.PaymentMethods
            .Where(pm => pm.UserId == userId)
            .OrderByDescending(pm => pm.IsDefault)
            .ThenByDescending(pm => pm.CreatedAt)
            .ToListAsync();
    }

    public async Task SavePaymentMethodAsync(PaymentMethod paymentMethod)
    {
        var existing = await _context.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.AuthorizationCode == paymentMethod.AuthorizationCode);

        if (existing != null)
            return;

        var hasExisting = await _context.PaymentMethods.AnyAsync(pm => pm.UserId == paymentMethod.UserId);
        paymentMethod.Id = Guid.NewGuid();
        paymentMethod.IsDefault = !hasExisting;
        paymentMethod.CreatedAt = DateTime.UtcNow;

        _context.PaymentMethods.Add(paymentMethod);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePaymentMethodAsync(Guid id)
    {
        var pm = await _context.PaymentMethods.FindAsync(id)
            ?? throw new KeyNotFoundException($"Payment method {id} not found.");

        _context.PaymentMethods.Remove(pm);
        await _context.SaveChangesAsync();
    }

    public async Task SetDefaultPaymentMethodAsync(Guid id)
    {
        var target = await _context.PaymentMethods.FindAsync(id)
            ?? throw new KeyNotFoundException($"Payment method {id} not found.");

        var current = await _context.PaymentMethods
            .Where(pm => pm.UserId == target.UserId && pm.IsDefault)
            .ToListAsync();

        foreach (var pm in current)
            pm.IsDefault = false;

        target.IsDefault = true;
        await _context.SaveChangesAsync();
    }
}