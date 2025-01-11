using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.API.Extensions;

public static class SettingsInjectionExtension
{
    public static void ConfigureSettings(this IServiceCollection services, IConfiguration config)
    {
        //services.Configure<TestSettings>(config.GetSection("test"));
    }
}