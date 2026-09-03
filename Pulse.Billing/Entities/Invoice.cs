using Pulse.Billing.Enums;

namespace Pulse.Billing.Entities;

public class Invoice
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid SubscriptionId { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;

    public InvoiceStatus Status { get; set; } // Success | Failed | Attention 
    public InvoiceType Type { get; set; } // Initial | Renewal
    public string InvoiceCode { get; set; } = string.Empty; // what provider Ids the invicoe with

    //public string? PaymentReference { get; set; } // Stripe/PayPal transaction ID //
    public DateTime IssuedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}
