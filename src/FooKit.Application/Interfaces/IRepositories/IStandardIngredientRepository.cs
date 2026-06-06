using FooKit.Domain.Entities;
using FooKit.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace FooKit.Application.Interfaces.IRepositories
{
    public interface IStandardIngredientRepository : IGenericRepository<StandardIngredient>
    {
        Task<(IEnumerable<StandardIngredient> Items, int TotalCount)> GetPaginatedAsync(int page, int size, string? search, IngredientCategory? category);
        Task<int> GetAffiliateProductsCountAsync(Guid id);
        Task<int> GetIngredientDictionariesCountAsync(Guid id);
        Task<IEnumerable<StandardIngredient>> GetIngredientsForSyncAsync(int maxLinks, DateTime cutoffTime, bool forceSyncAll, string targetIngredientId);
    }
}
