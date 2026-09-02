using System.ComponentModel.DataAnnotations.Schema;

namespace Pulse.Billing.Entities;

public class Subscription
{
    public Guid Id { get; set; } //internal use by my DB
    public Guid UserId { get; set; }
    public string? EmailToken { get; set; } // required alongside PaystackSubscriptionCode for the disable-subscription call
    public SubscriptionPlan Plan { get; set; } // Free | Pro

    [NotMapped]
    public int EndpointLimit => Plan switch
    {
        SubscriptionPlan.Free => 3,
        SubscriptionPlan.Pro => int.MaxValue,
        _ => 0
    };
    [NotMapped]
    public decimal MonthlyPrice => Plan switch
    {
        SubscriptionPlan.Free => 0m,
        SubscriptionPlan.Pro => 29m,
        _ => 0m
    };
    public string? PaystackSubscriptionCode { get; set; } // Paystack's reference .this is one that actually Id's my subscription from procviders side( for reccurring arrangement purposes)
    public DateTime StartedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }//...to track validity of subscription
    public bool CancelAtPeriodEnd { get; set; }//will auto sweep at period
    public DateTime? GracePeriodEndsAt { get; set; }// keep retrying via the renewal sweep
    public bool IsActive { get; set; }
}
