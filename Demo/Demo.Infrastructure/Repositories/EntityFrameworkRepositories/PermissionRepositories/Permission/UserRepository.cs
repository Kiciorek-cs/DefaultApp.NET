using Demo.BLL.Interfaces.Repositories.EntityFrameworkRepositories.PermissionRepositories.Permission;
using Demo.Domain.Entities.Permission;
using Demo.Infrastructure.Persistence.DatabaseContext.EntityFrameworkContext.Permission;

namespace Demo.Infrastructure.Repositories.EntityFrameworkRepositories.PermissionRepositories.Permission;

public class UserRepository : Repository<PermissionContext, User>, IUserRepository
{
    public UserRepository(PermissionContext context) : base(context)
    {
    }
}