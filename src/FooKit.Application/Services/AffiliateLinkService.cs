using AutoMapper;
using MyProject.Application.DTOs.AffiliateProductDtos;
using MyProject.Application.DTOs.Common;
using MyProject.Application.Interfaces.IServices;
using MyProject.Application.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyProject.Application.Services
{
    public class AffiliateLinkService : IAffiliateLinkService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AffiliateLinkService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AffiliateLinkDto>> GetAffiliateLinksAsync(int page, int size, bool? isActive, Guid? ingredientId)
        {
            var (items, totalCount) = await _unitOfWork.AffiliateProducts.GetPaginatedAsync(page, size, isActive, ingredientId);

            return new PagedResult<AffiliateLinkDto>
            {
                Items = _mapper.Map<IEnumerable<AffiliateLinkDto>>(items),
                Page = page,
                Size = size,
                TotalCount = totalCount
            };
        }

        public async Task<bool> ToggleStatusAsync(Guid id, ToggleAffiliateStatusDto dto)
        {
            var affiliateProduct = await _unitOfWork.AffiliateProducts.GetByIdAsync(id);
            if (affiliateProduct == null)
            {
                return false;
            }

            affiliateProduct.IsActive = dto.IsActive;
            _unitOfWork.AffiliateProducts.Update(affiliateProduct);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
