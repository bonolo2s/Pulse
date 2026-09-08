using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Entities;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Payments.Interfaces;

namespace Pulse.Billing.Services;

public class VerifyFallbackSweepService : IVerifyFallbackSweepService
{
    private readonly BillingDbContext _context;
    private readonly IPaymentProvider _paymentProvider;
    private readonly IBillingService _billingService;
    private readonly IBillingEventWriter _eventWriter;
    private readonly ILogger<VerifyFallbackSweepService> _logger;
    private readonly TimeSpan _stuckThreshold;

    public VerifyFallbackSweepService(
        BillingDbContext context,
        IPaymentProvider paymentProvider,
        IBillingService billingService,
        IBillingEventWriter eventWriter,
        ILogger<VerifyFallbackSweepService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _paymentProvider = paymentProvider;
        _billingService = billingService;
        _eventWriter = eventWriter;
        _logger = logger;
        _stuckThreshold = TimeSpan.FromMinutes(configuration.GetValue<int>("Billing:VerifyFallbackThresholdMinutes", 15));
    }

    public async Task SweepAsync(CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow - _stuckThreshold;

        var stuckReferences = await _context.BillingEvents
            .Where(e => e.EventType == BillingEventType.PaymentInitiated && e.ReceivedAt <= cutoff)
            .Select(e => new { e.PaymentReference, e.UserId }) //pull n drop everything else
            .Where(x => !_context.BillingEvents
                .Any(e2 => e2.PaymentReference == x.PaymentReference &&
                    (e2.EventType == BillingEventType.PaymentSuccessful || e2.EventType == BillingEventType.PaymentFailed)))// has been processed.
            .Distinct()
            .ToListAsync(cancellationToken);

        foreach (var stuck in stuckReferences)
        {
            if (stuck.PaymentReference == null || stuck.UserId == null) continue;

            await _eventWriter.LogEventAsync(
                eventType: BillingEventType.VerificationTimeout,
                source: BillingEventSource.VerifyFallback,
                userId: stuck.UserId,
                paystackEventId: null,
                paymentReference: stuck.PaymentReference,
                payload: null,
                previousStatus: BillingEventType.PaymentInitiated,
                newStatus: BillingEventType.VerificationTimeout);

            try
            {
                var result = await _paymentProvider.VerifyTransaction(stuck.PaymentReference);

                await _eventWriter.LogEventAsync(
                    eventType: BillingEventType.PaymentVerificationFallback,
                    source: BillingEventSource.VerifyFallback,
                    userId: stuck.UserId,
                    paystackEventId: null,
                    paymentReference: stuck.PaymentReference,
                    payload: null,
                    previousStatus: BillingEventType.VerificationTimeout,
                    newStatus: BillingEventType.PaymentVerificationFallback);

                await _billingService.ProcessPaymentResultAsync(
                    result.Reference,
                    null,
                    result.Status,
                    result.Channel,
                    null,
                    stuck.UserId.Value,
                    result.Authorization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Verify fallback failed for reference {Reference}", stuck.PaymentReference);
                // TODO: repeated verify failures for the same reference need visibility, not silent retry forever.
            }
        }
    }
}