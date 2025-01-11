using Demo.BLL.Features.Role.Commands.Create;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Validators.Resource;
using FluentValidation;

namespace Demo.BLL.Validators.Role.Commands;

public class CreateCommandValidator : CustomValidator<CreateCommand>
{
    public CreateCommandValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        var roleCommonValidator = new RoleCommonValidator(permissionUnitOfWork);
        var resourceCommonValidator = new ResourceCommonValidator(permissionUnitOfWork);

        RuleFor(x => x.Name)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "Name", context));

        RuleFor(x => x.Name)
            .Custom((x, context) => roleCommonValidator.RoleExistByName(x, context));

        RuleFor(x => x.Description)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "Description", context));

        RuleFor(x => x.Resources)
            .Custom((x, context) => resourceCommonValidator.ResourceExistByIdList(x, context));

        RuleFor(x => x.IsDefaultRole)
            .Custom((x, context) => roleCommonValidator.CheckIfDefaultRoleAlreadyExist(x, context));
    }
}