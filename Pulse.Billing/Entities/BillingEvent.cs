namespace Pulse.Billing.Entities;

public class BillingEvent
{
    public Guid Id { get; set; }
    public string PaystackEventId { get; init; } = string.Empty; // unique, enforces idempotency //
    public BillingEventType EventType { get; init; }//
    public Guid? UserId { get; init; } // nullable — may not resolve if payload is malformed/unmatched
    public string Payload { get; init; } = string.Empty; // raw JSON, dont ever mod this
    public DateTime ReceivedAt { get; init; }
    public DateTime? ProcessedAt { get; set; }
    public bool Processed { get; set; }
}