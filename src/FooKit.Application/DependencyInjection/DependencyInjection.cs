using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MyProject.Application.Interfaces.IServices;
using MyProject.Application.Mappings;
using MyProject.Application.Services;
using MyProject.Application.Validators;

namespace MyProject.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(LoginRequestValidator).Assembly);
        
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
