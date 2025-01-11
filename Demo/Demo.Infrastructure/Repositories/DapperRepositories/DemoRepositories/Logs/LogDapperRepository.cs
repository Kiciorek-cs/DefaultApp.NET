using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Demo.BLL.Interfaces.Repositories.DapperRepositories.DemoRepositories.Logs;
using Demo.Domain.Entities.Demo;
using Demo.Infrastructure.Persistence.DatabaseContext.DapperContext;
using Demo.Infrastructure.Repositories.DapperRepositories.QueriesHelpers;

namespace Demo.Infrastructure.Repositories.DapperRepositories.DemoRepositories.Logs;

public class LogDapperRepository : ILogDapperRepository
{
    private readonly DemoDapperContext _context;

    public LogDapperRepository(DemoDapperContext context)
    {
        _context = context;
    }

    public async Task<int> InsertLog(Log measurement, CancellationToken cancellationToken = default)
    {
        await using var connection = _context.CreateDemoConnection();

        await connection.OpenAsync(cancellationToken);

        var command = LogQueryBuilder.InsertLog(measurement);
        var id = connection.QuerySingle<int>(command);

        await connection.CloseAsync();

        return id;
    }
}