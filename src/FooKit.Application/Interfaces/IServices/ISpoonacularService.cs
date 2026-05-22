using System.Collections.Generic;
using System.Threading.Tasks;
using MyProject.Application.DTOs.SpoonacularDtos;

namespace MyProject.Application.Interfaces.IServices
{
    public interface ISpoonacularService
    {
        Task<List<SpoonacularRecipeDto>> SearchRecipesAsync(string equipment, string diet, int limit = 3);
    }
}
