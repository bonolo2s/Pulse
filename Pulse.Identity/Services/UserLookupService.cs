using Microsoft.EntityFrameworkCore;
using Pulse.Identity.DataAccess;
using Pulse.Shared.Interfaces;
namespace Pulse.Identity.Services
{
    public class UserLookupService : IUserLookupService
    {
        private readonly IdentityDbContext _context;
        public UserLookupService(IdentityDbContext context)
        {
            _context = context;
        }
        public async Task<Dictionary<Guid, string>> GetEmailsByUserIdsAsync(IEnumerable<Guid> userIds)
        {
            return await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Email);
        }
    }
}