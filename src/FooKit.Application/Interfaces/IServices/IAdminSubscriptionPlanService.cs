using System;
using System.Threading.Tasks;
using FooKit.Application.DTOs.AdminDtos;
using FooKit.Application.DTOs.Common;
using FooKit.Application.DTOs.SubscriptionDtos;

namespace FooKit.Application.Interfaces.IServices
{
    public interface IAdminSubscriptionPlanService
    {
        Task<PagedResult<SubscriptionPlanDto>> GetSubscriptionPlansAsync(GetSubscriptionPlansRequestDto request);
        Task<SubscriptionPlanDto> CreateSubscriptionPlanAsync(CreateSubscriptionPlanDto request);
        Task<SubscriptionPlanDto> UpdateSubscriptionPlanAsync(Guid id, UpdateSubscriptionPlanDto request);
        Task DeleteSubscriptionPlanAsync(Guid id);
    }
}
