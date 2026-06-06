using System;
using System.Threading.Tasks;
using FooKit.Domain.Entities;

namespace FooKit.Application.Interfaces.IRepositories
{
    public interface IUserSubscriptionRepository : IGenericRepository<UserSubscription>
    {
        Task<UserSubscription?> GetActiveSubscriptionAsync(Guid userId);
    }
}
