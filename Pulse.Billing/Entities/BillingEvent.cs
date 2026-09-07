namespace Pulse.Billing.Entities;

public class BillingEvent
{
    public Guid Id { get; set; }
    //public Guid? PaymentId { get; init; }
    public Guid? UserId { get; init; } 
    //public Payment? Payment { get; init; }
    public string? PaystackEventId { get; init; }// outer idempotency gate
    public string? PaymentReference { get; init; }//transaction im tracking
    public BillingEventType EventType { get; init; }//initiator, not producer.
    public BillingEventSource Source { get; init; }

    public string? Payload { get; init; } // raw JSON, dont ever mod this
    public BillingEventType? PreviousStatus { get; init; }
    public BillingEventType? NewStatus { get; init; }
    public DateTime ReceivedAt { get; init; }
    //public DateTime? ProcessedAt { get; set; }
    //public bool? Processed { get; set; } // null = not applicable (log-only event), false = pending, true = done
}