using AutoMapper;
using FooKit.Application.DTOs.AuthDtos;
using FooKit.Application.DTOs.IngredientDtos;
using FooKit.Application.DTOs.AiDictionaryDtos;
using FooKit.Application.DTOs.AffiliateProductDtos;
using FooKit.Domain.Entities;
using System.Linq;
using FooKit.Application.DTOs.UserDtos;
using FooKit.Application.DTOs.AdminDtos;

namespace FooKit.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterRequest, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<StandardIngredient, StandardIngredientDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()));

            CreateMap<CreateIngredientDto, StandardIngredient>()
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<UpdateIngredientDto, StandardIngredient>()
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<IngredientDictionary, AiDictionaryDto>()
                .ForMember(dest => dest.StandardIngredientName, opt => opt.MapFrom(src => src.StandardIngredient != null ? src.StandardIngredient.Name : string.Empty));

            CreateMap<AffiliateProduct, AffiliateLinkDto>()
                .ForMember(dest => dest.StandardIngredientName, opt => opt.MapFrom(src => src.StandardIngredient != null ? src.StandardIngredient.Name : string.Empty))
                .ForMember(dest => dest.CurrentPriceAmount, opt => opt.MapFrom(src => src.CurrentPrice != null ? src.CurrentPrice.Amount : 0))
                .ForMember(dest => dest.CurrentPriceCurrency, opt => opt.MapFrom(src => src.CurrentPrice != null ? src.CurrentPrice.Currency : "VND"));

            CreateMap<User, UserProfileResponse>();

            CreateMap<User, UserAdminResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SubscriptionStatus, opt => opt.MapFrom(src => 
                    src.UserSubscriptions != null 
                    ? src.UserSubscriptions.Where(s => s.IsActive).OrderByDescending(s => s.EndDate).FirstOrDefault() 
                    : null));

            CreateMap<UserSubscription,UserAdminSubscriptionStatusDto>()
                .ForMember(dest => dest.IsPremium, opt => opt.MapFrom(src => src.EndDate > System.DateTime.UtcNow))
                .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.SubscriptionPlan != null ? src.SubscriptionPlan.PlanName : null))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate));
        }
    }
}
