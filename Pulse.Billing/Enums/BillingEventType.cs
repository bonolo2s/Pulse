namespace Pulse.Billing.Entities;

public enum BillingEventType
{
    PaymentInitiated,
    PaymentPending,
    PaymentProviderReferenceCreated,
    PaymentProcessing,
    PaymentSuccessful,
    PaymentFailed,

    WebhookRejected,           // failed signature/IP check — security-relevant, currently silently 401
    DuplicateEventReceived,
    PaymentVerificationTimeout,// Pending too long, no webhook arrived, triggers fallback verify
    PaymentVerificationFallback,// BE polled Paystack directly to resolve a stuck Pending**

    ChargeSuccess,
    ChargeFailed,

    SubscriptionEnable,
    SubscriptionDisable,
    SubscriptionCreate,
    SubscriptionNotRenew,
    SubscriptionExpiringCards,

    InvoiceCreate,
    InvoiceUpdate,
    Unknown
}