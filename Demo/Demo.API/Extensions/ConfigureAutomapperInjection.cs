using Demo.BLL.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.API.Extensions;

public static class AutoMapperInjectionExtension
{
    public static void ConfigureAutoMapperInjection(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Startup));

        var mapperConfig = AutoMappingProfile.Configure();

        var mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);
    }
}