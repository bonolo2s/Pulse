namespace Pulse.Billing.Interfaces;
public interface ISubscriptionExpirySweepService
{
    Task SweepAsync(CancellationToken cancellationToken);
}