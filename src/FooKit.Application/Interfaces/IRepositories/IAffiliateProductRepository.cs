using MyProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyProject.Application.Interfaces.IRepositories
{
    public interface IAffiliateProductRepository : IGenericRepository<AffiliateProduct>
    {
        Task<(IEnumerable<AffiliateProduct> Items, int TotalCount)> GetPaginatedAsync(int page, int size, bool? isActive, Guid? ingredientId);
    }
}
