namespace Pulse.Shared.Interfaces;

public interface IBillingValidator
{
    Task ValidateEndpointLimitAsync(Guid userId, int currentEndpointCount); //will be passed to AddEndpoint handler to inter cept
}