using DotNetEnv;

namespace FooKit.API.Extensions
{
    public static class ConfigurationExtensions
    {
        public static WebApplicationBuilder AddEnvironmentConfig(this WebApplicationBuilder builder)
        {
            Env.Load();
            builder.Configuration.AddEnvironmentVariables();
            return builder;
        }
    }
}
