using Microsoft.EntityFrameworkCore;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Interfaces;

namespace Pulse.Billing.Services;

public class ExpiringCardCheckService : IExpiringCardCheckService
{
    private readonly BillingDbContext _context;

    public ExpiringCardCheckService(BillingDbContext context)
    {
        _context = context;
    }

    public async Task CheckAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var expiringCards = await _context.PaymentMethods
            .Where(pm => pm.ExpiryMonth == now.Month && pm.ExpiryYear == now.Year)
            .ToListAsync(cancellationToken);

        // TODO: notify each user in expiringCards — email/in-app, not built yet
    }
}