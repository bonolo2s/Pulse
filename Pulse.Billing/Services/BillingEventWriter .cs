using Microsoft.EntityFrameworkCore;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Entities;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Services;

public class BillingEventWriter : IBillingEventWriter
{
    private readonly BillingDbContext _context;

    public BillingEventWriter(BillingDbContext context)
    {
        _context = context;
    }

    public async Task LogEventAsync(
        BillingEventType eventType,
        BillingEventSource source,
        Guid? paymentId,
        Guid? userId,
        string? paystackEventId,
        string? payload,
        string? previousStatus,
        string? newStatus,
        bool? processed = null)
    {
        var billingEvent = new BillingEvent
        {
            Id = Guid.NewGuid(),
            EventType = eventType,
            Source = source,
            PaymentId = paymentId,
            UserId = userId,
            PaystackEventId = paystackEventId,
            Payload = payload,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            ReceivedAt = DateTime.UtcNow,
            Processed = RequiresProcessing(eventType) ? false : null
        };

        _context.BillingEvents.Add(billingEvent);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasProcessedEventAsync(string paystackEventId)
    {
        return await _context.BillingEvents
            .AnyAsync(e => e.PaystackEventId == paystackEventId);
    }
    private static bool RequiresProcessing(BillingEventType eventType) => eventType switch
    {
        BillingEventType.PaymentSuccessful => true,
        BillingEventType.PaymentFailed => true,
        BillingEventType.ChargeSuccess => true,
        BillingEventType.ChargeFailed => true,
        BillingEventType.SubscriptionEnable => true,
        BillingEventType.SubscriptionDisable => true,
        _ => false
    };
}