using MyProject.Application.DTOs.Common;
using MyProject.Application.DTOs.IngredientDtos;
using System;
using System.Threading.Tasks;

namespace MyProject.Application.Interfaces.IServices
{
    public interface IIngredientService
    {
        Task<PagedResult<StandardIngredientDto>> GetIngredientsAsync(int page, int size, string? search, string? category);
        Task<StandardIngredientDto> CreateIngredientAsync(CreateIngredientDto dto);
        Task<StandardIngredientDto> UpdateIngredientAsync(Guid id, UpdateIngredientDto dto);
        Task<bool> DeleteIngredientAsync(Guid id);
    }
}
