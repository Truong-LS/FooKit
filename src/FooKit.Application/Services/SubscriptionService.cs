using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using MyProject.Application.DTOs.SubscriptionDtos;
using MyProject.Application.Interfaces.IRepositories;
using MyProject.Application.Interfaces.IServices;

namespace MyProject.Application.Services
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
            return _mapper.Map<IEnumerable<SubscriptionPlanDto>>(plans);
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
    }
}
