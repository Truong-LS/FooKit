using System;
using System.Threading.Tasks;
using FooKit.Application.DTOs.HomepageDtos;

namespace FooKit.Application.Interfaces.IServices
{
    public interface IHomepageSuggestionService
    {
        Task<MealSuggestionResponseDto> GetMealSuggestionsAsync(Guid userId, string mealType);
    }
}
