namespace Pulse.Billing.Payments.Paystack.DTOs
{
    public class PaystackSubscriptionWebhookPayload
    {
        public string Event { get; set; } = string.Empty; // "subscription.create"
        public PaystackSubscriptionWebhookData Data { get; set; } = null!;
    }

    public class PaystackSubscriptionWebhookData
    {
        public string SubscriptionCode { get; set; } = string.Empty;
        public string EmailToken { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "active"
        public PaystackWebhookCustomer Customer { get; set; } = null!;
        public PaystackSubscriptionPlan Plan { get; set; } = null!;
        public PaystackAuthorization? Authorization { get; set; }
    }

    public class PaystackSubscriptionPlan
    {
        public string PlanCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string Interval { get; set; } = string.Empty;
    }
}