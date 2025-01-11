using System.Reflection;
using Demo.Domain.Entities.Permission;
using Microsoft.EntityFrameworkCore;

namespace Demo.Infrastructure.Persistence.DatabaseContext.EntityFrameworkContext.Permission;

public class PermissionContext : BaseContext
{
    public PermissionContext(DbContextOptions<PermissionContext> optionContext) : base(optionContext)
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Login> Logins { get; set; }
    public virtual DbSet<Token> Tokens { get; set; }
    public virtual DbSet<Resource> Resources { get; set; }
    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging(false);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
            type => type.Namespace != null && type.Namespace.EndsWith(".PermissionEntity"));
    }
}