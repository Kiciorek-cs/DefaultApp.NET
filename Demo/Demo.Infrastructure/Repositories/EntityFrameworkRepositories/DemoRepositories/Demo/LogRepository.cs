using Demo.BLL.Interfaces.Repositories.EntityFrameworkRepositories.DemoRepositories.Demo;
using Demo.Domain.Entities.Demo;
using Demo.Infrastructure.Persistence.DatabaseContext.EntityFrameworkContext.Demo;

namespace Demo.Infrastructure.Repositories.EntityFrameworkRepositories.DemoRepositories.Demo;

public class LogRepository : Repository<DemoContext, Log>, ILogRepository
{
    public LogRepository(DemoContext context) : base(context)
    {
    }
}