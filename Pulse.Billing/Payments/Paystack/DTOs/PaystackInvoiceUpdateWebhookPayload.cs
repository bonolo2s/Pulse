using Pulse.Billing.Payments.Paystack.DTOs;
using System.Text.Json.Serialization;

public class PaystackInvoiceUpdateWebhookPayload
{
    public string Event { get; set; } = string.Empty;
    public PaystackInvoiceUpdateWebhookData Data { get; set; } = null!;
}

public class PaystackInvoiceUpdateWebhookData
{
    [JsonPropertyName("invoice_code")]
    public string InvoiceCode { get; set; } = string.Empty;

    public int Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool Paid { get; set; }
    public string Currency { get; set; } = string.Empty;
    public PaystackWebhookCustomer Customer { get; set; } = null!;
}