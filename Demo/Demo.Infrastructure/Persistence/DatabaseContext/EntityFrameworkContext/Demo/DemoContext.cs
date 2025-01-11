using System.Reflection;
using Demo.Domain.Entities.Demo;
using Microsoft.EntityFrameworkCore;

namespace Demo.Infrastructure.Persistence.DatabaseContext.EntityFrameworkContext.Demo;

public class DemoContext : BaseContext
{
    public DemoContext(DbContextOptions<DemoContext> optionContext) : base(optionContext)
    {
    }

    public virtual DbSet<Log> Logs { get; set; }
    public virtual DbSet<BackgroundTask> BackgroundTasks { get; set; }
    public virtual DbSet<Country> Countries { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging(false);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
            type => type.Namespace != null && type.Namespace.EndsWith(".DemoEntity"));
    }
}