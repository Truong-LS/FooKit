using Microsoft.EntityFrameworkCore;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Domain.Entities;
using FooKit.Infrastructure.Data.DBContext;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FooKit.Infrastructure.Repositories
{
    public class UserSubscriptionRepository : GenericRepository<UserSubscription>, IUserSubscriptionRepository
    {
        public UserSubscriptionRepository(FooKitDbContext context) : base(context)
        {
        }

        public async Task<UserSubscription?> GetActiveSubscriptionAsync(Guid userId)
        {
            return await _context.UserSubscriptions
                .Include(x => x.SubscriptionPlan)
                .Where(x => x.UserId == userId && x.IsActive && x.EndDate > DateTime.UtcNow)
                .OrderByDescending(x => x.EndDate)
                .FirstOrDefaultAsync();
        }
    }
}
