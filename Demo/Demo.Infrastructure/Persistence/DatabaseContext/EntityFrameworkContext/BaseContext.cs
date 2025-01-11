using Microsoft.EntityFrameworkCore;

namespace Demo.Infrastructure.Persistence.DatabaseContext.EntityFrameworkContext;

public class BaseContext : DbContext
{
    public BaseContext(DbContextOptions optionContext) : base(optionContext)
    {
    }
}