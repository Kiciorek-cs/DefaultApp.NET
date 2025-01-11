using Demo.BLL.Interfaces.Repositories;
using FluentValidation;

namespace Demo.BLL.Validators.Role;

public class RoleCommonValidator
{
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public RoleCommonValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
    }

    public bool RoleExistById<T>(int id, ValidationContext<T> context)
    {
        var isExist =
            _permissionUnitOfWork.RoleRepository.GetAsync(x => x.Id == id).GetAwaiter().GetResult() is not null;

        if (!isExist)
        {
            context.AddCustomFailure("Role doesn't exist in system.", ValidationKeys.RoleNotExist);

            return false;
        }

        return true;
    }

    public bool RoleExistByName<T>(string name, ValidationContext<T> context, int? id = null)
    {
        var isExist = _permissionUnitOfWork.RoleRepository.GetAsync(y =>
                y.Name.ToLower() == name.Trim() &&
                (
                    id == null || y.Id != id
                )
            ).GetAwaiter()
            .GetResult() is null;

        if (!isExist)
        {
            context.AddCustomFailure($"You cannot attach a role that is already in the systems {name}.",
                ValidationKeys.RoleExist, "Name");

            return false;
        }

        return true;
    }

    public bool CheckIfDefaultRoleExist<T>(ValidationContext<T> context)
    {
        var isExist =
            _permissionUnitOfWork.RoleRepository.GetAsync(x => x.IsDefaultRole).GetAwaiter().GetResult() is not null;

        if (!isExist)
        {
            context.AddCustomFailure("Default role doesn't exist in system.", ValidationKeys.RoleNotExist);

            return false;
        }

        return true;
    }

    public bool CheckIfDefaultRoleAlreadyExist<T>(bool isDefaultRole, ValidationContext<T> context)
    {
        if (!isDefaultRole) return true;

        var isExist =
            _permissionUnitOfWork.RoleRepository.GetAsync(x => x.IsDefaultRole).GetAwaiter().GetResult() is not null;

        if (isExist)
        {
            context.AddCustomFailure("Default role already exist in system.", ValidationKeys.RoleExist);

            return false;
        }

        return true;
    }
}