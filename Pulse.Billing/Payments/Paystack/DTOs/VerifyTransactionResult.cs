namespace Pulse.Billing.Payments.Paystack.DTOs
{
    public record VerifyTransactionResult(
        string Reference,
        string Status,
        decimal Amount,
        string Currency,
        string? Channel,
        PaystackAuthorization? Authorization
    );
}