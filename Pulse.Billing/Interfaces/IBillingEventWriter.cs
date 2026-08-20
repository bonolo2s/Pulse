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
            Guid? paymentId,
            Guid? userId,
            string? paystackEventId,
            string? payload,
            string? previousStatus,
            string? newStatus);

        Task<bool> HasProcessedEventAsync(string paystackEventId);
    }
}
