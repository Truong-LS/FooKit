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
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IDishSuggestionService, DishSuggestionService>();
        services.AddScoped<IHomepageSuggestionService, HomepageSuggestionService>();
        services.AddScoped<IAdminDashboardService, AdminDashboardService>();
        services.AddScoped<IIngredientService, IngredientService>();
        services.AddScoped<IAiDictionaryService, AiDictionaryService>();
        services.AddScoped<IAffiliateLinkService, AffiliateLinkService>();
        services.AddScoped<IAffiliateSyncService, AffiliateSyncService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddSingleton<IHomepageCacheSignal, HomepageCacheSignal>();

        return services;
    }
}
