using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.Repositories.DapperRepositories.DemoRepositories.Logs;
using Demo.BLL.Interfaces.Repositories.EntityFrameworkRepositories.DemoRepositories.Demo;

namespace Demo.BLL.Interfaces.Repositories;

public interface IDemoUnitOfWork
{
    ILogRepository LogRepository { get; }
    IBackgroundTaskRepository BackgroundTaskRepository { get; }
    ICountryRepository CountryRepository { get; }

    ILogDapperRepository LogDapperRepository { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync();
    int Commit();
}