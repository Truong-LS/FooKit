using Microsoft.EntityFrameworkCore;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Domain.Entities;
using FooKit.Infrastructure.Data.DBContext;

namespace FooKit.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(FooKitDbContext context) : base(context) { }

        public override async Task<User?> GetByIdAsync(Guid id) =>
            await _dbSet.Include(u => u.Role).SingleOrDefaultAsync(u => u.Id == id);

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _dbSet.Include(u => u.Role).SingleOrDefaultAsync(u => u.Username == username);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _dbSet.Include(u => u.Role).SingleOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByUsernameOrEmailAsync(string identifier) =>
            await _dbSet.Include(u => u.Role).SingleOrDefaultAsync(u => u.Username == identifier || u.Email == identifier);

        public async Task<(System.Collections.Generic.IEnumerable<User> Users, int TotalCount)> GetUsersWithSubscriptionsAsync(string? search, bool? isPremium, bool? isActive, int page, int size)
        {
            var query = _dbSet.Include(u => u.UserSubscriptions).ThenInclude(us => us.SubscriptionPlan).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => (u.FullName != null && u.FullName.Contains(search)) || 
                                         (u.Email != null && u.Email.Contains(search)) ||
                                         u.Username.Contains(search));
            }

            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            if (isPremium.HasValue)
            {
                var now = DateTime.UtcNow;
                if (isPremium.Value)
                {
                    query = query.Where(u => u.UserSubscriptions.Any(s => s.EndDate > now && s.IsActive));
                }
                else
                {
                    query = query.Where(u => !u.UserSubscriptions.Any(s => s.EndDate > now && s.IsActive));
                }
            }

            int totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return (users, totalCount);
        }
    }
}
