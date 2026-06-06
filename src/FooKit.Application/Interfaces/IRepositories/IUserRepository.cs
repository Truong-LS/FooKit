using FooKit.Domain.Entities;

namespace FooKit.Application.Interfaces.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameOrEmailAsync(string identifier);
        Task<(IEnumerable<User> Users, int TotalCount)> GetUsersWithSubscriptionsAsync(string? search, bool? isPremium, bool? isActive, int page, int size);
    }
}
