namespace Pulse.Billing.Entities;

public enum PaystackEventType
{
    PaymentInitiated,
    PaymentPending,
    PaymentProviderReferenceCreated,
    PaymentProcessing,
    PaymentSuccessful,
    PaymentFailed,

    ChargeSuccess,
    ChargeFailed,

    SubscriptionEnable,
    SubscriptionDisable
}