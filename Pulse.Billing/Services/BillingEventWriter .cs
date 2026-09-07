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
        Guid? userId,
        string? paystackEventId,
        string? paymentReference,
        string? payload,
        BillingEventType? previousStatus,
        BillingEventType? newStatus)
    {
        var billingEvent = new BillingEvent
        {
            Id = Guid.NewGuid(),
            EventType = eventType,
            Source = source,
            UserId = userId,
            PaystackEventId = paystackEventId,
            PaymentReference = paymentReference,
            Payload = payload,
            PreviousStatus = previousStatus,
            NewStatus = newStatus,
            ReceivedAt = DateTime.UtcNow
        };

        _context.BillingEvents.Add(billingEvent);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasProcessedEventAsync(string paystackEventId)
    {
        return await _context.BillingEvents
            .AnyAsync(e => e.PaystackEventId == paystackEventId);
    }
    //private static bool RequiresProcessing(BillingEventType eventType) => eventType switch //sweeper will mark those that require processing in memomery
    //                                                                                       //e.g like pending too long
    //{
    //    BillingEventType.PaymentSuccessful => true,
    //    BillingEventType.PaymentFailed => true,
    //    BillingEventType.ChargeSuccess => true,
    //    BillingEventType.ChargeFailed => true,
    //    BillingEventType.SubscriptionEnable => true,
    //    BillingEventType.SubscriptionDisable => true,
    //    _ => false
    //};
}