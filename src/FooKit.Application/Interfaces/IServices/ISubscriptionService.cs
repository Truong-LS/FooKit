using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyProject.Application.DTOs.SubscriptionDtos;

namespace MyProject.Application.Interfaces.IServices
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync();
        Task<UserSubscriptionDto> GetCurrentSubscriptionAsync(Guid userId);
        Task<IEnumerable<PaymentHistoryDto>> GetPaymentHistoryAsync(Guid userId);
        Task CancelSubscriptionAsync(Guid userId);
    }
}
