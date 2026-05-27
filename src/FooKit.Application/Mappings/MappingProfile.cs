using AutoMapper;
using MyProject.Application.DTOs.AuthDtos;
using MyProject.Application.DTOs.IngredientDtos;
using MyProject.Application.DTOs.AiDictionaryDtos;
using MyProject.Application.DTOs.AffiliateProductDtos;
using MyProject.Domain.Entities;

namespace MyProject.Application.Mappings
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
