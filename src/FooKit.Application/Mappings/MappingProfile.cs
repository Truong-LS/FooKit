using AutoMapper;
using MyProject.Application.DTOs.AuthDtos;
using MyProject.Domain.Entities;

namespace MyProject.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterRequest, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        }
    }
}
