namespace Pulse.Billing.Interfaces;

public interface IExpiringCardCheckService
{
    Task CheckAsync(CancellationToken cancellationToken);
}