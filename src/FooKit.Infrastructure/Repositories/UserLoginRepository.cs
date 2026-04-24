using Microsoft.EntityFrameworkCore;
using MyProject.Application.Interfaces.IRepositories;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Data.DBContext;

namespace MyProject.Infrastructure.Repositories
{
    public class UserLoginRepository : IUserLoginRepository
    {
        private readonly ApplicationDbContext _context;

        public UserLoginRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserLogin?> FindAsync(string loginProvider, string providerKey) =>
            await _context.UserLogins
                .Include(ul => ul.User)
                .SingleOrDefaultAsync(ul => ul.LoginProvider == loginProvider && ul.ProviderKey == providerKey);

        public async Task<IEnumerable<UserLogin>> GetByUserIdAsync(Guid userId) =>
            await _context.UserLogins
                .Where(ul => ul.UserId == userId)
                .ToListAsync();

        public async Task AddAsync(UserLogin userLogin) =>
            await _context.UserLogins.AddAsync(userLogin);
    }
}
