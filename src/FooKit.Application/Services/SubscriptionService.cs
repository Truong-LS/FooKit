using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MyProject.Application.DTOs.SubscriptionDtos;
using MyProject.Application.Interfaces.IRepositories;
using MyProject.Application.Interfaces.IServices;

namespace MyProject.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync()
        {
            var plans = await _unitOfWork.SubscriptionPlans.GetAllAsync();
            return plans.Select(p => new SubscriptionPlanDto
            {
                Id = p.Id,
                PlanName = p.PlanName,
                Price = p.Price.Amount,
                Currency = p.Price.Currency,
                DurationInDays = p.DurationInDays,
                Features = string.IsNullOrEmpty(p.FeaturesJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(p.FeaturesJson) ?? new List<string>()
            });
        }

        public async Task<UserSubscriptionDto> GetCurrentSubscriptionAsync(Guid userId)
        {
            var activeSubscription = await _unitOfWork.UserSubscriptions.GetActiveSubscriptionAsync(userId);

            if (activeSubscription == null)
            {
                return new UserSubscriptionDto
                {
                    IsPremium = false,
                    PlanName = "Free"
                };
            }

            return new UserSubscriptionDto
            {
                IsPremium = true,
                PlanName = activeSubscription.SubscriptionPlan?.PlanName ?? "Premium",
                StartDate = activeSubscription.StartDate,
                EndDate = activeSubscription.EndDate,
                DaysRemaining = (int)(activeSubscription.EndDate - DateTime.UtcNow).TotalDays
            };
        }

        public async Task<IEnumerable<PaymentHistoryDto>> GetPaymentHistoryAsync(Guid userId)
        {
            var payments = await _unitOfWork.Payments.GetByUserIdAsync(userId);

            return payments.Select(p => new PaymentHistoryDto
            {
                Id = p.Id,
                PlanName = p.SubscriptionPlan?.PlanName ?? "Unknown",
                Amount = p.Amount,
                Status = p.Status.ToString(),
                TransactionRef = p.TransactionRef,
                CreatedAt = p.CreatedAt,
                PaidAt = p.PaidAt
            });
        }

        public async Task CancelSubscriptionAsync(Guid userId)
        {
            var activeSubscription = await _unitOfWork.UserSubscriptions.GetActiveSubscriptionAsync(userId);
            if (activeSubscription != null)
            {
                activeSubscription.IsActive = false;
                _unitOfWork.UserSubscriptions.Update(activeSubscription);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
