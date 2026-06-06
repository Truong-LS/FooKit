using FooKit.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FooKit.Application.Interfaces.IRepositories
{
    public interface IIngredientDictionaryRepository : IGenericRepository<IngredientDictionary>
    {
        Task<(IEnumerable<IngredientDictionary> Items, int TotalCount)> GetPaginatedAsync(int page, int size, string? searchRawText);
    }
}
