using Microsoft.EntityFrameworkCore;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Domain.Entities;
using FooKit.Domain.Enums;
using FooKit.Infrastructure.Data.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FooKit.Infrastructure.Repositories
{
    public class StandardIngredientRepository : GenericRepository<StandardIngredient>, IStandardIngredientRepository
    {
        public StandardIngredientRepository(FooKitDbContext context) : base(context) { }

        public async Task<(IEnumerable<StandardIngredient> Items, int TotalCount)> GetPaginatedAsync(int page, int size, string? search, IngredientCategory? category)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }

            if (category.HasValue)
            {
                query = query.Where(x => x.Category == category.Value);
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            return (items, totalCount);
        }

        public async Task<int> GetAffiliateProductsCountAsync(Guid id)
        {
            return await _context.AffiliateProducts.CountAsync(x => x.StandardIngredientId == id);
        }

        public async Task<int> GetIngredientDictionariesCountAsync(Guid id)
        {
            return await _context.IngredientDictionaries.CountAsync(x => x.StandardIngredientId == id);
        }

        public async Task<IEnumerable<StandardIngredient>> GetIngredientsForSyncAsync(int maxLinks, DateTime cutoffTime, bool forceSyncAll, string targetIngredientId)
        {
            var query = _dbSet.Include(si => si.AffiliateProducts).AsQueryable();

            if (!string.IsNullOrEmpty(targetIngredientId) && Guid.TryParse(targetIngredientId, out var parsedId))
            {
                query = query.Where(si => si.Id == parsedId);
            }

            if (!forceSyncAll)
            {
                query = query.Where(si =>
                    si.AffiliateProducts.Count(ap => ap.IsActive) < maxLinks ||
                    si.AffiliateProducts
                        .Where(ap => ap.IsActive)
                        .OrderByDescending(ap => ap.LastUpdatedPriceAt)
                        .Select(ap => ap.LastUpdatedPriceAt)
                        .FirstOrDefault() < cutoffTime);
            }

            return await query.ToListAsync();
        }
    }
}
