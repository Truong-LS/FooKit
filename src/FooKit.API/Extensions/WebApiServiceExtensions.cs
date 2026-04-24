using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using MyProject.API.Filters; // Filters are located in this folder
using FluentValidation.AspNetCore;

namespace MyProject.API.Extensions
{
    public static class WebApiServiceExtensions
    {
        public static IServiceCollection AddWebAPIServices(this IServiceCollection services)
        {
            // CORS Policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Rate Limiting
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("FixedPolicy", opt =>
                {
                    opt.PermitLimit = 100;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 2;
                });
            });

            // Output Caching (Replaces MemoryCache + Custom Filter)
            services.AddOutputCache(options =>
            {
                options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromMinutes(5)));
            });

            services.AddScoped<OwnerActionFilter>();

            services.AddControllers();
            
            // Re-adding the FluentValidation AutoValidation originally in the API project
            // because it's a presentation concern (integrating with MVC)
            services.AddFluentValidationAutoValidation();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddOpenApi();
            services.AddHealthChecks();

            return services;
        }
    }
}