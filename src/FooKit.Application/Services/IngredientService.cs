using AutoMapper;
using MyProject.Application.DTOs.Common;
using MyProject.Application.DTOs.IngredientDtos;
using MyProject.Application.Interfaces.IRepositories;
using MyProject.Application.Interfaces.IServices;
using MyProject.Domain.Entities;
using MyProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyProject.Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public IngredientService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<StandardIngredientDto>> GetIngredientsAsync(int page, int size, string? search, string? category)
        {
            IngredientCategory? parsedCategory = null;
            if (!string.IsNullOrWhiteSpace(category))
            {
                parsedCategory = ParseCategory(category);
            }

            var (items, totalCount) = await _unitOfWork.StandardIngredients.GetPaginatedAsync(page, size, search, parsedCategory);

            return new PagedResult<StandardIngredientDto>
            {
                Items = _mapper.Map<IEnumerable<StandardIngredientDto>>(items),
                Page = page,
                Size = size,
                TotalCount = totalCount
            };
        }

        public async Task<StandardIngredientDto> CreateIngredientAsync(CreateIngredientDto dto)
        {
            var entity = _mapper.Map<StandardIngredient>(dto);
            entity.Category = ParseCategory(dto.Category);

            await _unitOfWork.StandardIngredients.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<StandardIngredientDto>(entity);
        }

        public async Task<StandardIngredientDto> UpdateIngredientAsync(Guid id, UpdateIngredientDto dto)
        {
            var entity = await _unitOfWork.StandardIngredients.GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Ingredient not found.");
            }

            _mapper.Map(dto, entity);
            entity.Category = ParseCategory(dto.Category);

            _unitOfWork.StandardIngredients.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<StandardIngredientDto>(entity);
        }

        public async Task<bool> DeleteIngredientAsync(Guid id)
        {
            var entity = await _unitOfWork.StandardIngredients.GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }

            // Check references in AffiliateProduct
            var affiliateCount = await _unitOfWork.StandardIngredients.GetAffiliateProductsCountAsync(id);
            if (affiliateCount > 0)
            {
                throw new InvalidOperationException($"Cannot delete because there are currently {affiliateCount} Shopee affiliate links associated with this ingredient.");
            }

            // Check references in IngredientDictionary
            var dictionaryCount = await _unitOfWork.StandardIngredients.GetIngredientDictionariesCountAsync(id);
            if (dictionaryCount > 0)
            {
                throw new InvalidOperationException($"Cannot delete because there are currently {dictionaryCount} AI dictionary mappings associated with this ingredient.");
            }

            // Perform Soft Delete
            entity.IsDeleted = true;
            _unitOfWork.StandardIngredients.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private IngredientCategory ParseCategory(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return IngredientCategory.DairyAndOther;

            if (Enum.TryParse<IngredientCategory>(value, true, out var parsed))
            {
                return parsed;
            }

            var lower = value.ToLowerInvariant().Trim();
            if (lower == "thịt & hải sản" || lower == "thit & hai san" || lower.Contains("thit") || lower.Contains("hai san"))
                return IngredientCategory.MeatAndSeafood;
            if (lower == "tinh bột" || lower == "tinh bot" || lower.Contains("tinh"))
                return IngredientCategory.Starch;
            if (lower == "gia vị" || lower == "gia vi" || lower.Contains("gia"))
                return IngredientCategory.Spice;
            if (lower == "rau củ quả" || lower == "rau cu qua" || lower.Contains("rau") || lower.Contains("qua") || lower.Contains("trai"))
                return IngredientCategory.VegetablesAndFruits;

            return IngredientCategory.DairyAndOther;
        }
    }
}
