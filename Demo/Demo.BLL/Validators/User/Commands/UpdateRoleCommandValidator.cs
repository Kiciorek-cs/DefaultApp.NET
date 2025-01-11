using Demo.BLL.Features.User.Commands.UpdateRole;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Validators.Role;
using FluentValidation;

namespace Demo.BLL.Validators.User.Commands;

public class UpdateRoleCommandValidator : CustomValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        var userCommonValidator = new UserCommonValidator(permissionUnitOfWork);
        var roleCommonValidator = new RoleCommonValidator(permissionUnitOfWork);

        RuleFor(x => x.Id)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "Id", context));

        RuleFor(x => x.Id)
            .Custom((x, context) => userCommonValidator.UserExistById(x, context));

        RuleFor(x => x.RoleId)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "RoleId", context));

        RuleFor(x => x.RoleId)
            .Custom((x, context) => roleCommonValidator.RoleExistById(x, context));
    }
}