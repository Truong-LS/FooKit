using FooKit.Domain.Entities;

namespace FooKit.Application.Interfaces.IRepositories
{
    public interface IUserLoginRepository
    {
        Task<UserLogin?> FindAsync(string loginProvider, string providerKey);
        Task<IEnumerable<UserLogin>> GetByUserIdAsync(Guid userId);
        Task AddAsync(UserLogin userLogin);
    }
}
