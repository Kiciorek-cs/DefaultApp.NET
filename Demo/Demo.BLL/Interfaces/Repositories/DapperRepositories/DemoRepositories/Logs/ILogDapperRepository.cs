using System.Threading;
using System.Threading.Tasks;
using Demo.Domain.Entities.Demo;

namespace Demo.BLL.Interfaces.Repositories.DapperRepositories.DemoRepositories.Logs;

public interface ILogDapperRepository
{
    Task<int> InsertLog(Log measurement, CancellationToken cancellationToken = default);
}