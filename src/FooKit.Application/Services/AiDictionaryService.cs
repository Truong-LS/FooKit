using AutoMapper;
using MyProject.Application.DTOs.AiDictionaryDtos;
using MyProject.Application.DTOs.Common;
using MyProject.Application.Interfaces.IServices;
using MyProject.Application.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyProject.Application.Services
{
    public class AiDictionaryService : IAiDictionaryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AiDictionaryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AiDictionaryDto>> GetAiDictionaryAsync(int page, int size, string? searchRawText)
        {
            var (items, totalCount) = await _unitOfWork.IngredientDictionaries.GetPaginatedAsync(page, size, searchRawText);

            return new PagedResult<AiDictionaryDto>
            {
                Items = _mapper.Map<IEnumerable<AiDictionaryDto>>(items),
                Page = page,
                Size = size,
                TotalCount = totalCount
            };
        }

        public async Task<bool> UpdateMappingAsync(Guid id, UpdateAiDictionaryDto dto)
        {
            var record = await _unitOfWork.IngredientDictionaries.GetByIdAsync(id);
            if (record == null)
            {
                return false;
            }

            // Verify if the standard ingredient exists
            var ingredient = await _unitOfWork.StandardIngredients.GetByIdAsync(dto.NewStandardIngredientId);
            if (ingredient == null)
            {
                throw new KeyNotFoundException("Standard ingredient mapping target not found.");
            }

            record.StandardIngredientId = dto.NewStandardIngredientId;
            _unitOfWork.IngredientDictionaries.Update(record);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteKeywordAsync(Guid id)
        {
            var record = await _unitOfWork.IngredientDictionaries.GetByIdAsync(id);
            if (record == null)
            {
                return false;
            }

            _unitOfWork.IngredientDictionaries.Remove(record);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
