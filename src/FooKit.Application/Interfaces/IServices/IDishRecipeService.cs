using System;
using System.Threading.Tasks;
using FooKit.Application.DTOs.DishDtos;

namespace FooKit.Application.Interfaces.IServices
{
    public interface IDishRecipeService
    {
        Task<DishRecipeDetailDto> GetRecipeDetailAsync(Guid dishCacheId);
    }
}
