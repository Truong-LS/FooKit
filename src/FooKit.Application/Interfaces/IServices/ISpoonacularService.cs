using System.Collections.Generic;
using System.Threading.Tasks;
using FooKit.Application.DTOs.SpoonacularDtos;

namespace FooKit.Application.Interfaces.IServices
{
    public interface ISpoonacularService
    {
        Task<List<SpoonacularRecipeDto>> SearchRecipesAsync(string equipment, string diet, int limit = 3);
    }
}
