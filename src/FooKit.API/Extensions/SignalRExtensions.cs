namespace MyProject.API.Extensions;

public static class SignalRExtensions
{
    /// <summary>
    /// Registers SignalR services with configured options.
    /// </summary>
    public static IServiceCollection AddSignalRServices(this IServiceCollection services)
    {
        services.AddSignalR(options =>
        {
            // Enable detailed error messages (disable in production)
            options.EnableDetailedErrors = true;
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
