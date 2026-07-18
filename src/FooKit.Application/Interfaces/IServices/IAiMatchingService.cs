using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FooKit.Application.DTOs.IngredientDtos;
using FooKit.Application.DTOs.DishDtos;

namespace FooKit.Application.Interfaces.IServices
{
    public interface IAiMatchingService
    {
        Task<Dictionary<string, Guid?>> MatchIngredientsAsync(List<string> rawEnglishIngredients, List<StandardIngredientDto> standardIngredients);
        Task<AiGeneratedRecipeDto> GenerateRecipeAsync(string dishName, List<string> ingredients);
    }
}
