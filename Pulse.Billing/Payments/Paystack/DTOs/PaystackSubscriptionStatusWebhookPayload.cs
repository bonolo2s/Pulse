public class PaystackSubscriptionStatusWebhookPayload
{
    public string Event { get; set; } = string.Empty; // "subscription.not_renew" or "subscription.disable"
    public PaystackSubscriptionStatusWebhookData Data { get; set; } = null!;
}

public class PaystackSubscriptionStatusWebhookData
{
    public string SubscriptionCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}