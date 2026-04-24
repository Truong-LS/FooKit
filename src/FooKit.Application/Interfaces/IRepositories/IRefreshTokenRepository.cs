using MyProject.Domain.Entities;

namespace MyProject.Application.Interfaces.IRepositories
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token, Guid userId);
        Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId);
    }
}
