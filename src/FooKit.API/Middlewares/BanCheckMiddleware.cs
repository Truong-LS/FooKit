using System;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using FooKit.Application.Interfaces.IRepositories;

namespace FooKit.API.Middlewares
{
    public class BanCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public BanCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IMemoryCache memoryCache)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdString = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (Guid.TryParse(userIdString, out var userId))
                {
                    string cacheKey = $"UserActiveStatus_{userId}";
                    
                    if (!memoryCache.TryGetValue(cacheKey, out bool isActive))
                    {
                        using var scope = context.RequestServices.CreateScope();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        
                        var user = await unitOfWork.Users.GetByIdAsync(userId);
                        isActive = user?.IsActive ?? false;
                        
                        memoryCache.Set(cacheKey, isActive, TimeSpan.FromMinutes(15));
                    }

                    if (!isActive)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        var response = new { status = "Failed", message = "Your account has been banned." };
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
