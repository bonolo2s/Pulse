namespace Pulse.Billing.DTOs;

public class SyncPaymentWebhookRequest
{
    public string PaymentReference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}