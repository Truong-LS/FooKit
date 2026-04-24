using Microsoft.EntityFrameworkCore;
using MyProject.Application.Interfaces.IRepositories;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Data.DBContext;

namespace MyProject.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _dbSet.SingleOrDefaultAsync(u => u.Username == username);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _dbSet.SingleOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByUsernameOrEmailAsync(string identifier) =>
            await _dbSet.SingleOrDefaultAsync(u => u.Username == identifier || u.Email == identifier);
    }
}
