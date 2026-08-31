namespace Pulse.Billing.Interfaces;
public interface IVerifyFallbackSweepService
{
    Task SweepAsync(CancellationToken cancellationToken);
}