using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using MyProject.Application.DTOs.SubscriptionDtos;
using MyProject.Domain.Entities;

namespace MyProject.Application.Mappings
{
    public class SubscriptionProfile : Profile
    {
        public SubscriptionProfile()
        {
            CreateMap<SubscriptionPlan, SubscriptionPlanDto>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Price.Currency))
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => 
                    string.IsNullOrEmpty(src.FeaturesJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(src.FeaturesJson, (JsonSerializerOptions?)null) ?? new List<string>()));

            CreateMap<UserSubscription, UserSubscriptionDto>()
                .ForMember(dest => dest.IsPremium, opt => opt.MapFrom(src => src.IsActive && src.EndDate > DateTime.UtcNow))
                .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.SubscriptionPlan != null ? src.SubscriptionPlan.PlanName : "Unknown"))
                .ForMember(dest => dest.DaysRemaining, opt => opt.MapFrom(src => (int)(src.EndDate - DateTime.UtcNow).TotalDays));

            CreateMap<Payment, PaymentHistoryDto>()
                .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.SubscriptionPlan != null ? src.SubscriptionPlan.PlanName : "Unknown"))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
