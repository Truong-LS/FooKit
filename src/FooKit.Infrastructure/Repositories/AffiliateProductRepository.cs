using Microsoft.EntityFrameworkCore;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Domain.Entities;
using FooKit.Infrastructure.Data.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FooKit.Infrastructure.Repositories
{
    public class AffiliateProductRepository : GenericRepository<AffiliateProduct>, IAffiliateProductRepository
    {
        public AffiliateProductRepository(FooKitDbContext context) : base(context) { }

        public async Task<(IEnumerable<AffiliateProduct> Items, int TotalCount)> GetPaginatedAsync(int page, int size, bool? isActive, Guid? ingredientId)
        {
            var query = _dbSet.Include(x => x.StandardIngredient).AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            if (ingredientId.HasValue)
            {
                query = query.Where(x => x.StandardIngredientId == ingredientId.Value);
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            return (items, totalCount);
        }
    }
}
