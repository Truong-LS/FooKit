using Microsoft.EntityFrameworkCore;
using MyProject.Application.Interfaces.IRepositories;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Data.DBContext;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProject.Infrastructure.Repositories
{
    public class IngredientDictionaryRepository : GenericRepository<IngredientDictionary>, IIngredientDictionaryRepository
    {
        public IngredientDictionaryRepository(FooKitDbContext context) : base(context) { }

        public async Task<(IEnumerable<IngredientDictionary> Items, int TotalCount)> GetPaginatedAsync(int page, int size, string? searchRawText)
        {
            var query = _dbSet.Include(x => x.StandardIngredient).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchRawText))
            {
                query = query.Where(x => x.RawKeywordFromApi.Contains(searchRawText));
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            return (items, totalCount);
        }
    }
}
