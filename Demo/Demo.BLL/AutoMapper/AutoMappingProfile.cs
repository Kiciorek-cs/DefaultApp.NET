using AutoMapper;
using Demo.BLL.AutoMapper.Profiles;

namespace Demo.BLL.AutoMapper;

public static class AutoMappingProfile
{
    public static MapperConfiguration Configure()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
            cfg.AddProfile<BackgroundTaskProfile>();
            cfg.AddProfile<ResourceProfile>();
            cfg.AddProfile<RoleProfile>();
            cfg.AddProfile<CountryProfile>();
            cfg.AddProfile<LogProfile>();
        });
        return config;
    }
}