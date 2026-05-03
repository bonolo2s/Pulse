namespace Pulse.Billing.Entities;

public class Invoice
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public InvoiceStatus Status { get; set; } // Paid | Failed | Pending
    public string? PaymentReference { get; set; } // Stripe/PayPal transaction ID
    public DateTime IssuedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}
