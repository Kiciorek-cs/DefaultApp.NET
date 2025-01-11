using Demo.API.ConfigurationBuilder;
using Demo.BLL.Interfaces.Integrations.Email.Client;
using Demo.BLL.Interfaces.Integrations.Email.Notifications;
using Demo.BLL.Interfaces.Integrations.Http.Client;
using Demo.BLL.Interfaces.Integrations.Http.DemoService;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Services.BackgroundTaskService;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Interfaces.Services.EndlessMethodServices;
using Demo.BLL.Interfaces.Services.ExcelServices;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.BLL.Interfaces.Services.PermissionServices;
using Demo.BLL.Interfaces.Services.TaskManagers;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.BLL.Services.BackgroundTaskService;
using Demo.BLL.Services.Clock;
using Demo.BLL.Services.EndlessMethodServices;
using Demo.BLL.Services.ExcelServices;
using Demo.BLL.Services.Logs;
using Demo.BLL.Services.PermissionServices;
using Demo.BLL.Services.TaskManagers;
using Demo.BLL.Services.TokenServices;
using Demo.Domain.ConfigurationModels.StaticModels;
using Demo.Infrastructure.Persistence.DatabaseContext.DapperContext;
using Demo.Infrastructure.Persistence.DatabaseContext.EntityFrameworkContext.Demo;
using Demo.Infrastructure.Persistence.DatabaseContext.EntityFrameworkContext.Permission;
using Demo.Infrastructure.Repositories;
using Demo.Integration.Email.Client;
using Demo.Integration.Email.Services.Notifications;
using Demo.Integration.Http.Client;
using Demo.Integration.Http.Services.DemoService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.API.Extensions;

public static class ServiceInjectionExtension
{
    public static void ConfigureDbInjection(this Startup startup, IServiceCollection services, IClock clock)
    {
        startup.ConfigureDemoDb(services, clock);
        startup.ConfigurePermissionDb(services, clock);
    }

    public static void ConfigureServicesInjection(this Startup startup, IServiceCollection services, IClock clock)
    {
        services.AddScoped<IClock, UtcClock>();
        services.AddScoped<IActionBlockManager, ActionBlockManager>();
        services.AddScoped<IBackgroundTaskService, BackgroundTaskService>();
        services.AddScoped<IToken, Token>();
        services.AddScoped<ITokenDataExtractor, TokenDataExtractor>();
        services.AddHttpContextAccessor();
        services.AddScoped<IPermission, Permission>();
        services.AddScoped<ILogServices, LogServices>();
        services.AddTransient<IHttpClient, HttpClientAdapter>();
        services.AddTransient<IDemoService, DemoService>();
        services.AddTransient<IDoSomethingEndlessTask, DoSomethingEndlessTask>();
        services.AddScoped<IExcelServices, ExcelServices>();

        SetStaticProps(clock);

        ConfigureEmail(startup, services);
    }

    private static void SetStaticProps(IClock clock)
    {
        ErrorCustomConfigurationModel.LogsPath = CustomConfigurationBuilder.GetLogsPath(clock);
    }

    private static void ConfigureDemoDb(this Startup startup, IServiceCollection services, IClock clock)
    {
        //var connectionString = CustomConfigurationBuilder.CreateDemoDatabaseConfiguration(clock);

        var connectionString = startup.Configuration.GetConnectionStringToDemoDb();

        services.AddDbContext<DemoContext>(options =>
        {
            options.UseLazyLoadingProxies();
            options.UseSqlServer(connectionString);
        }, ServiceLifetime.Transient);

        services.AddSingleton(rep => new DemoDapperContext(connectionString));

        services.AddScoped<IDemoUnitOfWork>(rep => new DemoUnitOfWork(rep.GetService<DemoContext>(),
            rep.GetService<DemoDapperContext>(), rep.GetService<IHttpContextAccessor>()));
    }

    private static void ConfigurePermissionDb(this Startup startup, IServiceCollection services, IClock clock)
    {
        //var connectionString = CustomConfigurationBuilder.CreatePermissionDatabaseConfiguration(clock);

        var connectionString = startup.Configuration.GetConnectionStringToPermissionDb();

        services.AddDbContext<PermissionContext>(options =>
        {
            options.UseLazyLoadingProxies();
            options.UseSqlServer(connectionString);
        }, ServiceLifetime.Transient);

        services.AddScoped<IPermissionUnitOfWork>(rep => new PermissionUnitOfWork(rep.GetService<PermissionContext>()));
    }

    private static void ConfigureEmail(this Startup startup, IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailMessageService, EmailMessageService>();
    }

    public static string GetConnectionStringToDemoDb(this IConfiguration configuration)
    {
        return configuration.GetConnectionString("Demo");
    }

    public static string GetConnectionStringToPermissionDb(this IConfiguration configuration)
    {
        return configuration.GetConnectionString("Permission");
    }
}