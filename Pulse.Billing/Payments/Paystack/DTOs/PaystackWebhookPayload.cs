namespace Pulse.Billing.Payments.Paystack.DTOs
{
    public class PaystackWebhookPayload
    {
        public string Event { get; set; } = string.Empty; //identifies one specific webhook delivery
        public PaystackWebhookData Data { get; set; } = null!;
    }

    public class PaystackWebhookData
    {
        public long Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;//from chcekout in remains same for same payment attempt
        public int Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? Channel { get; set; } //card/bank
        public PaystackWebhookCustomer Customer { get; set; } = null!;
        public DateTime? PaidAt { get; set; }
        public PaystackAuthorization? Authorization { get; set; }
    }

    public class PaystackWebhookCustomer
    {
        public string Email { get; set; } = string.Empty;
        public string CustomerCode { get; set; } = string.Empty;
    }

    public class PaystackAuthorization //Paystacks reusable-charge object for both cards n Efts = PaymentMethod in gen
    {
        public string AuthorizationCode { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty; // "visa", "mastercard" etc
        public string Last4 { get; set; } = string.Empty;
        public string ExpMonth { get; set; } = string.Empty;
        public string ExpYear { get; set; } = string.Empty;
        public string Bank { get; set; } = string.Empty;
        public bool Reusable { get; set; }
    }
}