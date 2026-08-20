using System.ComponentModel.DataAnnotations.Schema;

namespace Pulse.Billing.Entities;

public class Subscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public SubscriptionPlan Plan { get; set; } // Free | Pro

    [NotMapped]
    public int EndpointLimit => Plan switch
    {
        SubscriptionPlan.Free => 3,
        SubscriptionPlan.Pro => int.MaxValue,
        _ => 0
    };

    public string? PaystackSubscriptionCode { get; set; } // Paystack's reference, null while on Free
    public DateTime StartedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}
