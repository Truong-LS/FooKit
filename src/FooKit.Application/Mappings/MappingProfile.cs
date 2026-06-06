using AutoMapper;
using FooKit.Application.DTOs.AuthDtos;
using FooKit.Application.DTOs.IngredientDtos;
using FooKit.Application.DTOs.AiDictionaryDtos;
using FooKit.Application.DTOs.AffiliateProductDtos;
using FooKit.Domain.Entities;

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
        }
    }
}
