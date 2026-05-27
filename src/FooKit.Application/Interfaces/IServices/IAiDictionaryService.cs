using MyProject.Application.DTOs.AiDictionaryDtos;
using MyProject.Application.DTOs.Common;
using System;
using System.Threading.Tasks;

namespace MyProject.Application.Interfaces.IServices
{
    public interface IAiDictionaryService
    {
        Task<PagedResult<AiDictionaryDto>> GetAiDictionaryAsync(int page, int size, string? searchRawText);
        Task<bool> UpdateMappingAsync(Guid id, UpdateAiDictionaryDto dto);
        Task<bool> DeleteKeywordAsync(Guid id);
    }
}
