using System;
using System.Threading.Tasks;
using MyProject.Application.DTOs.HomepageDtos;

namespace MyProject.Application.Interfaces.IServices
{
    public interface IHomepageSuggestionService
    {
        Task<HomepageSuggestionResponseDto> GetDailySuggestionsAsync(Guid userId);
    }
}
