using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyProject.Application.DTOs.IngredientDtos;

namespace MyProject.Application.Interfaces.IServices
{
    public interface IAiMatchingService
    {
        Task<Dictionary<string, Guid?>> MatchIngredientsAsync(List<string> rawEnglishIngredients, List<StandardIngredientDto> standardIngredients);
    }
}
