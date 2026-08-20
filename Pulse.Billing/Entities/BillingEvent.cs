namespace Pulse.Billing.Entities;

public class BillingEvent
{
    public Guid Id { get; set; }
    public Guid? PaymentId { get; init; }
    public Guid? UserId { get; init; } // nullable — may not resolve if payload is malformed/unmatched
    public Payment? Payment { get; init; }
    public string? PaystackEventId { get; init; }// unique EventId from paystack
    public BillingEventType EventType { get; init; }//
    public BillingEventSource Source { get; init; }

    public string? Payload { get; init; } // raw JSON, dont ever mod this
    public string? PreviousStatus { get; init; }
    public string? NewStatus { get; init; }
    public DateTime ReceivedAt { get; init; }
    public DateTime? ProcessedAt { get; set; }
    public bool? Processed { get; set; } // null = not applicable (log-only event), false = pending, true = done
}