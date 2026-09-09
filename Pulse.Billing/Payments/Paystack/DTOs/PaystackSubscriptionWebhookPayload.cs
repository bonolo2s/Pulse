using System.Text.Json.Serialization;

namespace Pulse.Billing.Payments.Paystack.DTOs
{
    public class PaystackSubscriptionWebhookPayload
    {
        public string Event { get; set; } = string.Empty;
        public PaystackSubscriptionWebhookData Data { get; set; } = null!;
    }

    public class PaystackSubscriptionWebhookData
    {
        [JsonPropertyName("subscription_code")]
        public string SubscriptionCode { get; set; } = string.Empty;

        [JsonPropertyName("email_token")]
        public string EmailToken { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
        public PaystackWebhookCustomer Customer { get; set; } = null!;
        public PaystackSubscriptionPlan Plan { get; set; } = null!;
        public PaystackAuthorization? Authorization { get; set; }
    }

    public class PaystackSubscriptionPlan
    {
        [JsonPropertyName("plan_code")]
        public string PlanCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string Interval { get; set; } = string.Empty;
    }
}