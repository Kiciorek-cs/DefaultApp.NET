using Demo.BLL.Interfaces.Repositories.EntityFrameworkRepositories.DemoRepositories.Demo;
using Demo.Domain.Entities.Demo;
using Demo.Infrastructure.Persistence.DatabaseContext.EntityFrameworkContext.Demo;

namespace Demo.Infrastructure.Repositories.EntityFrameworkRepositories.DemoRepositories.Demo;

public class BackgroundTaskRepository : Repository<DemoContext, BackgroundTask>, IBackgroundTaskRepository
{
    public BackgroundTaskRepository(DemoContext context) : base(context)
    {
    }
}