using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using FooKit.Application.DTOs.SubscriptionDtos;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Entities;

namespace FooKit.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubscriptionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync()
        {
            var plans = await _unitOfWork.SubscriptionPlans.GetAllAsync();
            var activePlans = plans.Where(p => p.IsActive);
            return _mapper.Map<IEnumerable<SubscriptionPlanDto>>(activePlans);
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

            return _mapper.Map<UserSubscriptionDto>(activeSubscription);
        }

        public async Task<IEnumerable<PaymentHistoryDto>> GetPaymentHistoryAsync(Guid userId)
        {
            var payments = await _unitOfWork.Payments.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<PaymentHistoryDto>>(payments);
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

        public async Task<UserSubscription> GrantSubscriptionAsync(Guid userId, SubscriptionPlan plan)
        {
            var activeSub = await _unitOfWork.UserSubscriptions.GetActiveSubscriptionAsync(userId);
            
            var startDate = DateTime.UtcNow;
            if (activeSub != null && activeSub.IsActive && activeSub.EndDate > startDate)
            {
                startDate = activeSub.EndDate;
            }

            var newSub = new UserSubscription
            {
                UserId = userId,
                PlanId = plan.Id,
                StartDate = startDate,
                EndDate = startDate.AddDays(plan.DurationInDays),
                IsActive = true
            };

            await _unitOfWork.UserSubscriptions.AddAsync(newSub);
            await _unitOfWork.SaveChangesAsync();

            return newSub;
        }
    }
}
