using Microsoft.EntityFrameworkCore;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Domain.Entities;
using FooKit.Infrastructure.Data.DBContext;

namespace FooKit.Infrastructure.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(FooKitDbContext context) : base(context) { }

        public async Task<RefreshToken?> GetByTokenAsync(string token, Guid userId) =>
            await _dbSet.FirstOrDefaultAsync(rt => rt.Token == token && rt.UserId == userId);

        public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId) =>
            await _dbSet.Where(rt => rt.UserId == userId && !rt.IsRevoked).ToListAsync();
    }
}
