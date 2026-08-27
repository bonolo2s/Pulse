using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pulse.Billing.Commands;
using Pulse.Billing.DataAccess;
using Pulse.Billing.Enums;
using Pulse.Billing.Interfaces;
using Pulse.Shared.Interfaces;
namespace Pulse.Billing.Services;
public class SubscriptionRenewalSweepService : ISubscriptionRenewalSweepService
{
    private readonly BillingDbContext _context;
    private readonly IUserLookupService _userLookupService;
    private readonly IMediator _mediator;
    private readonly ILogger<SubscriptionRenewalSweepService> _logger;

    public SubscriptionRenewalSweepService(
        BillingDbContext context,
        IUserLookupService userLookupService,
        IMediator mediator,
        ILogger<SubscriptionRenewalSweepService> logger)
    {
        _context = context;
        _userLookupService = userLookupService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task SweepAsync(CancellationToken cancellationToken)
    {
        var dueSubscriptions = await _context.Subscriptions
            .Where(s => s.IsActive && !s.CancelAtPeriodEnd && s.Plan == SubscriptionPlan.Pro && s.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        if (dueSubscriptions.Count == 0)
            return;

        var userIds = dueSubscriptions.Select(s => s.UserId).Distinct();
        var emails = await _userLookupService.GetEmailsByUserIdsAsync(userIds);

        //for now Sequential processing with try catch rather that crash...// once we scale
        foreach (var subscription in dueSubscriptions)
        {
            if (!emails.TryGetValue(subscription.UserId, out var email))
                continue;

            try
            {
                await _mediator.Send(new RenewSubscriptionCommand(subscription.Id, email), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Renewal failed for subscription {SubscriptionId}", subscription.Id);
                // TODO: audit — need BillingEvent logging here for failure cases (card decline, provider timeout) so we can see why a renewal failed, not just that it did
            }
        }
    }
}