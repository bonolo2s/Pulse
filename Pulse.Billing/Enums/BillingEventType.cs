namespace Pulse.Billing.Entities;

public enum BillingEventType
{
    // State Driven Events
    // --------------------
    PaymentInitiated,// reference obtained from Paystack
    //PaymentPending,// waiting on webhook
    //PaymentProviderReferenceCreated,
    //PaymentProcessing,// webhook arrived n is beieng processed.
    PaymentSuccessful,// charge.success, verified
    PaymentFailed,// charge.success payload status = "failed"

    InitiationFailed,     // never got a reference — dead before Pending

    DuplicateEventReceived,
    VerificationTimeout, // no webhook arrived in time, fallback triggered
    PaymentVerificationFallback,// BE polled Paystack directly to resolve a stuck Pending**


    // Webhook event-triggers
    // ----------------------
    WebhookRejected,           // failed signature/IP check — security-relevant, currently silently 401

    ChargeSuccess,
    ChargeFailed,

    SubscriptionEnable,
    SubscriptionDisable,
    SubscriptionCreate,
    SubscriptionNotRenew,
    SubscriptionExpiringCards,
    SubscriptionCancellationRequested,

    InvoiceCreate,
    InvoiceUpdate,
    Unknown
}