using FooKit.Application.DTOs.AffiliateProductDtos;
using FooKit.Application.DTOs.Common;
using System;
using System.Threading.Tasks;

namespace FooKit.Application.Interfaces.IServices
{
    public interface IAffiliateLinkService
    {
        Task<PagedResult<AffiliateLinkDto>> GetAffiliateLinksAsync(int page, int size, bool? isActive, Guid? ingredientId);
        Task<bool> ToggleStatusAsync(Guid id, ToggleAffiliateStatusDto dto);
        Task<AffiliateLinkDto> CreateAffiliateLinkAsync(CreateAffiliateLinkDto request);
        Task<AffiliateLinkDto> UpdateAffiliateLinkAsync(Guid id, UpdateAffiliateLinkDto request);
    }
}
