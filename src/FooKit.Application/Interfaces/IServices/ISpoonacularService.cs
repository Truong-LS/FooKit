using System.Collections.Generic;
using System.Threading.Tasks;
using FooKit.Application.DTOs.SpoonacularDtos;

namespace FooKit.Application.Interfaces.IServices
{
    public interface ISpoonacularService
    {
        Task<List<SpoonacularRecipeDto>> SearchRecipesAsync(string equipment, string diet, string intolerances, string cuisine, string mealType, int limit = 5, int offset = 0);
    }
}
