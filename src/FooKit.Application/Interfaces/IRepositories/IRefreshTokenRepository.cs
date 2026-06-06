using FooKit.Domain.Entities;

namespace FooKit.Application.Interfaces.IRepositories
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token, Guid userId);
        Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId);
    }
}
