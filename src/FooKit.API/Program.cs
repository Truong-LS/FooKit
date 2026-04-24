using MyProject.API.Extensions;
using MyProject.API.Middlewares;
using MyProject.Application.DependencyInjection;
using MyProject.Infrastructure.DependencyInjection;
using Scalar.AspNetCore;

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

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();