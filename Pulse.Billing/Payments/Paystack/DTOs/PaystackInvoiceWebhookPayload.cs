using Pulse.Billing.Payments.Paystack.DTOs;

public class PaystackInvoiceWebhookPayload
{
    public string Event { get; set; } = string.Empty;
    public PaystackInvoiceWebhookData Data { get; set; } = null!;
}

public class PaystackInvoiceWebhookData
{
    public string InvoiceCode { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool Paid { get; set; }
    public DateTime? PaidAt { get; set; }
    public PaystackWebhookCustomer Customer { get; set; } = null!;
    public PaystackInvoiceSubscriptionRef Subscription { get; set; } = null!;
    public PaystackInvoiceTransactionRef Transaction { get; set; } = null!;
}

public class PaystackInvoiceSubscriptionRef
{
    public string SubscriptionCode { get; set; } = string.Empty;
}

public class PaystackInvoiceTransactionRef
{
    public string Reference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
}