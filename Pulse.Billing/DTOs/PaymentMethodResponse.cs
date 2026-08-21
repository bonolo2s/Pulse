using Pulse.Billing.Enums;

namespace Pulse.Billing.DTOs;

public class PaymentMethodResponse
{
    public Guid Id { get; set; }
    public PaymentMethodType Type { get; set; }
    public CardBrand? Brand { get; set; }
    public string? Last4 { get; set; }
    public int? ExpiryMonth { get; set; }
    public int? ExpiryYear { get; set; }
    public string? BankName { get; set; }
    public bool IsDefault { get; set; }
}