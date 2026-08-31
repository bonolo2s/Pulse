using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Enums;
using Pulse.Billing.Interfaces;
using Pulse.Billing.Payments.Interfaces;
namespace Pulse.Billing.Services;
public class VerifyFallbackSweepService : IVerifyFallbackSweepService
{
    private readonly BillingDbContext _context;
    private readonly IPaymentProvider _paymentProvider;
    private readonly IBillingService _billingService;
    private readonly ILogger<VerifyFallbackSweepService> _logger;
    private readonly TimeSpan _stuckThreshold;

    public VerifyFallbackSweepService(
        BillingDbContext context,
        IPaymentProvider paymentProvider,
        IBillingService billingService,
        ILogger<VerifyFallbackSweepService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _paymentProvider = paymentProvider;
        _billingService = billingService;
        _logger = logger;
        _stuckThreshold = TimeSpan.FromMinutes(configuration.GetValue<int>("Billing:VerifyFallbackThresholdMinutes", 15));
    }

    public async Task SweepAsync(CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow - _stuckThreshold;

        var stuckPayments = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Pending && p.CreatedAt <= cutoff && p.ProviderReference != null)
            .ToListAsync(cancellationToken);

        foreach (var payment in stuckPayments)
        {
            try
            {
                var result = await _paymentProvider.VerifyTransaction(payment.ProviderReference!);

                await _billingService.ProcessPaymentResultAsync(result.Reference, result.Status, result.Channel, result.Authorization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Verify fallback failed for payment {PaymentId}, reference {Reference}", payment.Id, payment.ProviderReference);
                // TODO: audit — repeated verify failures for the same payment need visibility, not silent retry forever.
            }
        }
    }
}