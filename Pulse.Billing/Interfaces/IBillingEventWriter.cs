using Pulse.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Billing.Interfaces
{
    public interface IBillingEventWriter
    {
        Task LogEventAsync(
        BillingEventType eventType,
        BillingEventSource source,
        Guid? userId,
        string? paystackEventId,
        string? paymentReference,
        string? payload,
        BillingEventType? previousStatus,
        BillingEventType? newStatus);

        Task<bool> HasProcessedEventAsync(string paystackEventId);
    }
}
