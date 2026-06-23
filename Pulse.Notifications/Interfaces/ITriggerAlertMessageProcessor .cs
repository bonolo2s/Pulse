namespace Pulse.Notifications.Interfaces;

public interface ITriggerAlertMessageProcessor
{
    Task ProcessAsync(string messageBody, CancellationToken cancellationToken);
}