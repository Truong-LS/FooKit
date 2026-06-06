using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FooKit.Application.DTOs.SubscriptionDtos;

namespace FooKit.Application.Interfaces.IServices
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync();
        Task<UserSubscriptionDto> GetCurrentSubscriptionAsync(Guid userId);
        Task<IEnumerable<PaymentHistoryDto>> GetPaymentHistoryAsync(Guid userId);
        Task CancelSubscriptionAsync(Guid userId);
    }
}
