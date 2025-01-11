using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Repositories.EntityFrameworkRepositories.PermissionRepositories.Permission;
using Demo.Infrastructure.Persistence.DatabaseContext.EntityFrameworkContext.Permission;
using Demo.Infrastructure.Repositories.EntityFrameworkRepositories.PermissionRepositories.Permission;

namespace Demo.Infrastructure.Repositories;

public class PermissionUnitOfWork : IPermissionUnitOfWork
{
    #region private

    private readonly PermissionContext _permissionContext;

    private ILoginRepository _loginRepository;
    private IResourceRepository _resourceRepository;
    private IUserRepository _userRepository;
    private ITokenRepository _tokenRepository;
    private IRoleRepository _roleRepository;

    #endregion

    public PermissionUnitOfWork(PermissionContext permissionContext)
    {
        _permissionContext = permissionContext;
    }

    public ILoginRepository LoginRepository =>
        _loginRepository ??= new LoginRepository(_permissionContext);

    public IResourceRepository ResourceRepository =>
        _resourceRepository ??= new ResourceRepository(_permissionContext);

    public IUserRepository UserRepository =>
        _userRepository ??= new UserRepository(_permissionContext);

    public ITokenRepository TokenRepository =>
        _tokenRepository ??= new TokenRepository(_permissionContext);

    public IRoleRepository RoleRepository =>
        _roleRepository ??= new RoleRepository(_permissionContext);

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _permissionContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RollbackAsync()
    {
        await _permissionContext.DisposeAsync();
    }

    public int Commit()
    {
        return _permissionContext.SaveChanges();
    }
}