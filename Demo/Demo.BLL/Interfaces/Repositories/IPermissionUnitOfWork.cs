using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.Repositories.EntityFrameworkRepositories.PermissionRepositories.Permission;

namespace Demo.BLL.Interfaces.Repositories;

public interface IPermissionUnitOfWork
{
    IUserRepository UserRepository { get; }
    IResourceRepository ResourceRepository { get; }
    ILoginRepository LoginRepository { get; }
    ITokenRepository TokenRepository { get; }
    IRoleRepository RoleRepository { get; }

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync();
    int Commit();
}