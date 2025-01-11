using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Repositories.DapperRepositories.DemoRepositories.Logs;
using Demo.BLL.Interfaces.Repositories.EntityFrameworkRepositories.DemoRepositories.Demo;
using Demo.Infrastructure.Persistence.DatabaseContext.DapperContext;
using Demo.Infrastructure.Persistence.DatabaseContext.EntityFrameworkContext.Demo;
using Demo.Infrastructure.Repositories.DapperRepositories.DemoRepositories.Logs;
using Demo.Infrastructure.Repositories.EntityFrameworkRepositories.DemoRepositories.Demo;
using Microsoft.AspNetCore.Http;

namespace Demo.Infrastructure.Repositories;

public class DemoUnitOfWork : IDemoUnitOfWork
{
    #region private

    private readonly DemoContext _demoContext;
    private readonly DemoDapperContext _demoDapperContext;
    private readonly IHttpContextAccessor _httpContextAccessor;


    private ILogRepository _logRepository;
    private IBackgroundTaskRepository _backgroundTaskRepository;
    private ICountryRepository _countryRepository;

    private ILogDapperRepository _logDapperRepository;

    #endregion

    public DemoUnitOfWork(DemoContext demoContext, DemoDapperContext dapperContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _demoContext = demoContext;
        _demoDapperContext = dapperContext;

        _httpContextAccessor = httpContextAccessor;
    }

    public ILogRepository LogRepository =>
        _logRepository ??= new LogRepository(_demoContext);

    public IBackgroundTaskRepository BackgroundTaskRepository =>
        _backgroundTaskRepository ??= new BackgroundTaskRepository(_demoContext);

    public ICountryRepository CountryRepository =>
        _countryRepository ??= new CountryRepository(_demoContext, _httpContextAccessor);


    public ILogDapperRepository LogDapperRepository =>
        _logDapperRepository ??= new LogDapperRepository(_demoDapperContext);


    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _demoContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RollbackAsync()
    {
        await _demoContext.DisposeAsync();
    }

    public int Commit()
    {
        return _demoContext.SaveChanges();
    }
}