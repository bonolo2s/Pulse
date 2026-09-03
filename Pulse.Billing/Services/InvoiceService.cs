using Microsoft.EntityFrameworkCore;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Entities;
using Pulse.Billing.Enums;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Services;

public class InvoiceService : IInvoiceService
{
    private readonly BillingDbContext _context;

    public InvoiceService(BillingDbContext context)
    {
        _context = context;
    }

    public async Task CreateInvoiceFromWebhookAsync(string subscriptionCode, string invoiceCode, int amount, string currency, string status, bool paid, DateTime? paidAt)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.PaystackSubscriptionCode == subscriptionCode)
            ?? throw new KeyNotFoundException($"Subscription with code {subscriptionCode} not found.");

        var parsedStatus = status.ToLowerInvariant() switch
        {
            "success" => InvoiceStatus.Success,
            "attention" => InvoiceStatus.Attention,
            "failed" => InvoiceStatus.Failed,
            _ => InvoiceStatus.Unknown
        };

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            UserId = subscription.UserId,
            SubscriptionId = subscription.Id,
            Amount = amount / 100m,
            Currency = currency,
            Status = parsedStatus,
            Type = InvoiceType.Renewal,
            InvoiceCode = invoiceCode,
            IssuedAt = DateTime.UtcNow,
            PaidAt = paidAt
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Invoice>> GetBillingHistoryAsync(Guid userId)
    {
        return await _context.Invoices
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.IssuedAt)
            .ToListAsync();
    }

    //public async Task<Invoice> CreatePendingInvoiceAsync(Guid userId, Guid subscriptionId, decimal amount, string currency, InvoiceType type)
    //{
    //    var invoice = new Invoice
    //    {
    //        Id = Guid.NewGuid(),
    //        UserId = userId,
    //        SubscriptionId = subscriptionId,
    //        Amount = amount,
    //        Currency = currency,
    //        Status = InvoiceStatus.Pending,
    //        Type = type,
    //        IssuedAt = DateTime.UtcNow
    //    };

    //    _context.Invoices.Add(invoice);
    //    await _context.SaveChangesAsync();

    //    return invoice;
    //}
}