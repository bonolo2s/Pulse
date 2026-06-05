using Pulse.Lambda.Models;

namespace Pulse.Lambda.Interfaces;

public interface IEndpointRepository
{
    Task<IEnumerable<EndpointForCheckDto>> GetEndpointsByIntervalAsync(int intervalSeconds);
}