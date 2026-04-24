using Microsoft.EntityFrameworkCore;
using MyProject.Application.Interfaces.IRepositories;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Data.DBContext;

namespace MyProject.Infrastructure.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context) { }

        public async Task<RefreshToken?> GetByTokenAsync(string token, Guid userId) =>
            await _dbSet.FirstOrDefaultAsync(rt => rt.Token == token && rt.UserId == userId);

        public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId) =>
            await _dbSet.Where(rt => rt.UserId == userId && !rt.IsRevoked).ToListAsync();
    }
}
