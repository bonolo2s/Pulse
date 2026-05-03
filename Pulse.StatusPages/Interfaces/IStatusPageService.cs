namespace Pulse.StatusPages.Interfaces;

public interface IStatusPageService
{
    Task<StatusPage> CreateStatusPageAsync(StatusPage statusPage);
    Task<StatusPage> UpdateStatusPageAsync(Guid id, StatusPage statusPage);
    Task DeleteStatusPageAsync(Guid id);
    Task<StatusPageEndpoint> AddEndpointToStatusPageAsync(StatusPageEndpoint statusPageEndpoint);
    Task RemoveEndpointFromStatusPageAsync(Guid statusPageEndpointId);
    Task<StatusPage> GetPublicStatusPageAsync(string slug);
    Task<StatusPage> GetPrivateStatusPageAsync(Guid statusPageId);
}