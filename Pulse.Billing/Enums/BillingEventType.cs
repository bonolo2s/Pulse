namespace Pulse.Billing.Entities;

public enum BillingEventType
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