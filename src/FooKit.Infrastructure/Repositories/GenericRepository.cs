using Microsoft.EntityFrameworkCore;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Infrastructure.Data.DBContext;
using System.Linq.Expressions;

namespace FooKit.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly FooKitDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(FooKitDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression) =>
            await _dbSet.Where(expression).ToListAsync();

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities) => await _dbSet.AddRangeAsync(entities);

        public void Update(T entity) => _dbSet.Update(entity);

        public void UpdateRange(IEnumerable<T> entities) => _dbSet.UpdateRange(entities);

        public void Remove(T entity) => _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);
    }
}
