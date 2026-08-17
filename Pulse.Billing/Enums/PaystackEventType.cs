namespace Pulse.Billing.Entities;

public enum BillingEventType
{
    PaymentInitiated,
    PaymentPending,
    PaymentSuccessful,
    PaymentFailed,

    ChargeSuccess,
    ChargeFailed,

    SubscriptionEnable,
    SubscriptionDisable
}