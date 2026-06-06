using FooKit.API.Extensions;
using FooKit.API.Hubs;
using FooKit.API.Middlewares;
using FooKit.API.Workers;
using FooKit.Application.Configuration;
using FooKit.Application.DependencyInjection;
using FooKit.Infrastructure.DependencyInjection;
using Scalar.AspNetCore;
using Hangfire;
using Hangfire.MemoryStorage;

var builder = WebApplication.CreateBuilder(args);

builder.AddEnvironmentConfig();

#region Architecture Layers
builder.Services.AddWebAPIServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
#endregion

#region Cross-cutting Concerns (Security, Exception Handling...)
builder.Services.AddGlobalExceptionHandler();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSignalRServices();
#endregion

#region Background Workers
builder.Services.Configure<AffiliateWorkerOptions>(
    builder.Configuration.GetSection(AffiliateWorkerOptions.SectionName));

// Bind AccessKey from environment variable
builder.Services.PostConfigure<AffiliateWorkerOptions>(options =>
{
    var accessKey = builder.Configuration["ACCESSTRADE_ACCESS_KEY"];
    if (!string.IsNullOrEmpty(accessKey))
    {
        options.AccessKey = accessKey;
    }
});

builder.Services.AddHostedService<AutoSearchAffiliateWorker>();

builder.Services.AddMemoryCache();

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseMemoryStorage());

builder.Services.AddHangfireServer();
#endregion

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "My .NET 9 API";
        options.Theme = ScalarTheme.Mars;
    });
}
app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseRateLimiter();

app.UseOutputCache();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<BanCheckMiddleware>();

app.MapHealthChecks("/health");

app.MapControllers();

#region SignalR Hub Endpoints
app.MapHub<NotificationHub>("/hubs/notification");
#endregion

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    // Authorization could be added here for Admin
});

app.Run();

