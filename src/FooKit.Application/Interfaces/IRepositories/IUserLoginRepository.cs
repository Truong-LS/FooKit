using MyProject.Domain.Entities;

namespace MyProject.Application.Interfaces.IRepositories
{
    public interface IUserLoginRepository
    {
        Task<UserLogin?> FindAsync(string loginProvider, string providerKey);
        Task<IEnumerable<UserLogin>> GetByUserIdAsync(Guid userId);
        Task AddAsync(UserLogin userLogin);
    }
}
