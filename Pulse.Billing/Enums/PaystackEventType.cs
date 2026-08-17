namespace Pulse.Billing.Entities;

public enum PaystackEventType
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