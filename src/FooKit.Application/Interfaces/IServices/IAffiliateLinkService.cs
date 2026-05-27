using MyProject.Application.DTOs.AffiliateProductDtos;
using MyProject.Application.DTOs.Common;
using System;
using System.Threading.Tasks;

namespace MyProject.Application.Interfaces.IServices
{
    public interface IAffiliateLinkService
    {
        Task<PagedResult<AffiliateLinkDto>> GetAffiliateLinksAsync(int page, int size, bool? isActive, Guid? ingredientId);
        Task<bool> ToggleStatusAsync(Guid id, ToggleAffiliateStatusDto dto);
    }
}
