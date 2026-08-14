using Pulse.Billing.Enums;

namespace Pulse.Billing.Entities;

public class PaymentMethod
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public PaymentMethodType Type { get; set; }

    // Card-specific — null when Type == Eft
    public CardBrand? Brand { get; set; }
    public string? Last4 { get; set; }
    public int? ExpiryMonth { get; set; }
    public int? ExpiryYear { get; set; }

    // EFT-specific — null when Type == Card
    public string? BankName { get; set; }

    public string AuthorizationCode { get; set; } = string.Empty; // token ref for auto sub reuse
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
}