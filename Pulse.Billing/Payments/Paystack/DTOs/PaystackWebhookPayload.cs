namespace Pulse.Billing.Payments.Paystack.DTOs
{
    public class PaystackWebhookPayload
    {
        public string Event { get; set; } = string.Empty;
        public PaystackWebhookData Data { get; set; } = null!;
    }

    public class PaystackWebhookData
    {
        public long Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? Channel { get; set; }
        public PaystackWebhookCustomer Customer { get; set; } = null!;
        public DateTime? PaidAt { get; set; }
    }

    public class PaystackWebhookCustomer
    {
        public string Email { get; set; } = string.Empty;
        public string CustomerCode { get; set; } = string.Empty;
    }
}