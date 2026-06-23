namespace Pulse.Observability.Interfaces;

public interface IRecordResultMessageProcessor
{
    Task ProcessAsync(string messageBody, CancellationToken cancellationToken);
}
