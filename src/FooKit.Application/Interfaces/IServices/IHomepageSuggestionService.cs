using System;
using System.Threading.Tasks;
using FooKit.Application.DTOs.HomepageDtos;

namespace FooKit.Application.Interfaces.IServices
{
    public interface IHomepageSuggestionService
    {
        Task<HomepageSuggestionResponseDto> GetDailySuggestionsAsync(Guid userId);
    }
}
