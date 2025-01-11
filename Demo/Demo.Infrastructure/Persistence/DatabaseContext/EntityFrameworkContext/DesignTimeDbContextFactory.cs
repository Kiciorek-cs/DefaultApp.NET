using System;
using System.IO;
using Demo.Infrastructure.Persistence.DatabaseContext.EntityFrameworkContext.Demo;
using Demo.Infrastructure.Persistence.DatabaseContext.EntityFrameworkContext.Permission;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Demo.Infrastructure.Persistence.DatabaseContext.EntityFrameworkContext;

public class DemoContextFactory : DesignTimeDbContextFactory<DemoContext>
{
}

public class PermissionContextFactory : DesignTimeDbContextFactory<PermissionContext>
{
}

public class DesignTimeDbContextFactory<T> : IDesignTimeDbContextFactory<T>
    where T : DbContext
{
    public T CreateDbContext(string[] args)
    {
        var contextName = GetContextNameFromArgs(args);

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Demo.API"))
            .AddJsonFile($"appsettings.{environment}.json", true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<T>();

        var connectionString = configuration.GetConnectionString(contextName == "DemoContext" ? "Demo" : "Permission");

        optionsBuilder.UseSqlServer(connectionString);

        var dbContext = (T)Activator.CreateInstance(
            typeof(T),
            optionsBuilder.Options);

        return dbContext;
    }

    private string GetContextNameFromArgs(string[] args)
    {
        for (var i = 0; i < args.Length - 1; i++)
            if (string.Equals(args[i], "-Context", StringComparison.CurrentCultureIgnoreCase))
                return args[i + 1];
        return null;
    }
}