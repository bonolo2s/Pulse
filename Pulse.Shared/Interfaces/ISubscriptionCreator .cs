namespace Pulse.Shared.Interfaces;

public interface ISubscriptionCreator
{
    Task CreateSubscriptionAsync(Guid userId);
}