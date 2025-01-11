using Demo.Domain.ConfigurationModels.DynamicModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.API.Extensions;

public static class AuthorizationDependencyInjection
{
    public static void AddCustomAuthorization(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<AuthorizeEnableSettings>(
            option => config.GetSection("AuthorizeEnabledSettings").Bind(option));
        services.Configure<JwtTokenSettings>(option => config.GetSection("JwtTokenSettings").Bind(option));
    }
}