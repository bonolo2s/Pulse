namespace Pulse.Shared.Interfaces
{
    public interface IUserLookupService
    {
        Task<Dictionary<Guid, string>> GetEmailsByUserIdsAsync(IEnumerable<Guid> userIds);
    }
}
