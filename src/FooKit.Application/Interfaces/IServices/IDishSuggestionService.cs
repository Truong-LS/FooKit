using System;
using System.Threading.Tasks;
using MyProject.Application.DTOs.DishDtos;

namespace MyProject.Application.Interfaces.IServices
{
    public interface IDishSuggestionService
    {
        Task<DishSuggestionResponseDto> GetSuggestionsAsync(Guid userId, DishSuggestionRequestDto request);
    }
}
