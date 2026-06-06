using System;
using System.Threading.Tasks;
using FooKit.Application.DTOs.AdminDtos;
using FooKit.Application.DTOs.Common;

namespace FooKit.Application.Interfaces.IServices
{
    public interface IAdminUserService
    {
        Task<PagedResult<UserAdminResponseDto>> GetUsersAsync(GetUsersRequestDto request);
        Task<UserAdminSubscriptionStatusDto> GrantPremiumAsync(Guid userId, GrantPremiumRequestDto request);
        Task ToggleBanAsync(Guid userId, ToggleBanRequestDto request);
    }
}
