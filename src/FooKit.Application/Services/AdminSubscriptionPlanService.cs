using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using FooKit.Application.DTOs.AdminDtos;
using FooKit.Application.DTOs.Common;
using FooKit.Application.DTOs.SubscriptionDtos;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Application.Interfaces.IServices;
using FooKit.Domain.Entities;
using FooKit.Domain.Exceptions;
using FooKit.Domain.ValueObjects;

namespace FooKit.Application.Services
{
    public class AdminSubscriptionPlanService : IAdminSubscriptionPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminSubscriptionPlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<SubscriptionPlanDto>> GetSubscriptionPlansAsync(GetSubscriptionPlansRequestDto request)
        {
            var allPlans = await _unitOfWork.SubscriptionPlans.GetAllAsync();
            var query = allPlans.AsQueryable();

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x => x.PlanName.ToLower().Contains(search));
            }

            var totalRecords = query.Count();
            var plans = query
                .OrderBy(x => x.Price.Amount)
                .Skip((request.Page - 1) * request.Size)
                .Take(request.Size)
                .ToList();

            return new PagedResult<SubscriptionPlanDto>
            {
                Items = _mapper.Map<IEnumerable<SubscriptionPlanDto>>(plans),
                TotalCount = totalRecords,
                Page = request.Page,
                Size = request.Size
            };
        }

        public async Task<SubscriptionPlanDto> CreateSubscriptionPlanAsync(CreateSubscriptionPlanDto request)
        {
            var plan = new SubscriptionPlan
            {
                PlanName = request.PlanName,
                Price = new Money(request.Price, request.Currency),
                DurationInDays = request.DurationInDays,
                FeaturesJson = JsonSerializer.Serialize(request.Features),
                IsActive = true
            };

            await _unitOfWork.SubscriptionPlans.AddAsync(plan);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SubscriptionPlanDto>(plan);
        }

        public async Task<SubscriptionPlanDto> UpdateSubscriptionPlanAsync(Guid id, UpdateSubscriptionPlanDto request)
        {
            var plan = await _unitOfWork.SubscriptionPlans.GetByIdAsync(id);
            if (plan == null)
            {
                throw new NotFoundException("Subscription plan not found.");
            }

            plan.PlanName = request.PlanName;
            plan.Price = new Money(request.Price, request.Currency);
            plan.DurationInDays = request.DurationInDays;
            plan.FeaturesJson = JsonSerializer.Serialize(request.Features);
            plan.IsActive = request.IsActive;

            _unitOfWork.SubscriptionPlans.Update(plan);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SubscriptionPlanDto>(plan);
        }

        public async Task DeleteSubscriptionPlanAsync(Guid id)
        {
            var plan = await _unitOfWork.SubscriptionPlans.GetByIdAsync(id);
            if (plan == null)
            {
                throw new NotFoundException("Subscription plan not found.");
            }

            plan.IsActive = false;
            _unitOfWork.SubscriptionPlans.Update(plan);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
