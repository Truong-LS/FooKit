using System;
using System.Threading.Tasks;
using FooKit.Application.DTOs.DishDtos;

namespace FooKit.Application.Interfaces.IServices
{
    public interface IDishSuggestionService
    {
        Task<DishSuggestionResponseDto> GetSuggestionsAsync(Guid userId, DishSuggestionRequestDto request);
    }
}
