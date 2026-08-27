namespace Pulse.Billing.Interfaces;
public interface ISubscriptionRenewalSweepService
{
    Task SweepAsync(CancellationToken cancellationToken);
}