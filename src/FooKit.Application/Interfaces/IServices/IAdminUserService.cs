using System;
using System.Threading.Tasks;
using MyProject.Application.DTOs.AdminDtos;
using MyProject.Application.DTOs.Common;

namespace MyProject.Application.Interfaces.IServices
{
    public interface IAdminUserService
    {
        Task<PagedResult<UserAdminResponseDto>> GetUsersAsync(GetUsersRequestDto request);
        Task<UserAdminSubscriptionStatusDto> GrantPremiumAsync(Guid userId, GrantPremiumRequestDto request);
        Task ToggleBanAsync(Guid userId, ToggleBanRequestDto request);
    }
}
