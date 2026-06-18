using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FooKit.Application.Interfaces.IRepositories;
using FooKit.Application.Interfaces.IServices;
using FooKit.Application.Configuration;
using FooKit.Domain.Entities;
using FooKit.Infrastructure.Data.DBContext;
using FooKit.Infrastructure.ExternalServices;
using FooKit.Infrastructure.Repositories;
using Polly;
using Polly.Extensions.Http;
namespace FooKit.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config["DB_CONNECTION_STRING"];
        services.AddDbContext<FooKitDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserLoginRepository, UserLoginRepository>();
        services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IGoogleAuthProvider, GoogleAuthProvider>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IVnPayService, VnPayService>();

        services.AddSingleton<WorkerHealthTracker>();
        services.AddScoped<IImageService, ImageService>();

        // Register Options
        services.Configure<SpoonacularOptions>(options =>
        {
            options.ApiKey = config["SPOONACULAR_API_KEY"] ?? string.Empty;
            options.BaseUrl = config["SPOONACULAR_BASE_URL"] ?? "https://api.spoonacular.com";
        });

        services.Configure<GeminiOptions>(options =>
        {
            options.ApiKey = config["GEMINI_API_KEY"] ?? string.Empty;
            options.BaseUrl = config["GEMINI_BASE_URL"] ?? "https://generativelanguage.googleapis.com";
            options.Model = config["GEMINI_MODEL"] ?? "gemini-2.5-flash-lite";
        });

        services.Configure<CloudinarySettings>(options =>
        {
            options.CloudName = config["CloudinarySettings:CloudName"] ?? string.Empty;
            options.ApiKey = config["CloudinarySettings:ApiKey"] ?? string.Empty;
            options.ApiSecret = config["CloudinarySettings:ApiSecret"] ?? string.Empty;
        });

        // Register Accesstrade HttpClient with Polly retry policy (exponential backoff)
        services.AddHttpClient<IProductSearchApiService, AccesstradeProductSearchService>()
            .AddPolicyHandler(GetRetryPolicy());

        // Register Spoonacular Client with Polly retry
        services.AddHttpClient<ISpoonacularService, SpoonacularService>()
            .AddPolicyHandler(GetRetryPolicy());

        // Register Gemini Client with Polly retry
        services.AddHttpClient<IAiMatchingService, GeminiMatchingService>()
            .AddPolicyHandler(GetRetryPolicy());

        return services;
    }

    /// <summary>
    /// Polly retry policy: retries up to 3 times with exponential backoff
    /// for transient HTTP errors (5xx) and 429 Too Many Requests.
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() // Handles HttpRequestException + 5xx
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests) // 429
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, _) =>
                {
                    // Log is available via the typed HttpClient's ILogger
                });
    }
}

