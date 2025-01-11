using Demo.BLL.Features.Role.Commands.Update;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Validators.Resource;
using FluentValidation;

namespace Demo.BLL.Validators.Role.Commands;

public class UpdateCommandValidator : CustomValidator<UpdateCommand>
{
    public UpdateCommandValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        var roleCommonValidator = new RoleCommonValidator(permissionUnitOfWork);
        var resourceCommonValidator = new ResourceCommonValidator(permissionUnitOfWork);

        RuleFor(x => x.Id)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "Id", context));

        RuleFor(x => x.Id)
            .Custom((x, context) => roleCommonValidator.RoleExistById(x, context));

        RuleFor(x => x.Name)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "Name", context));

        RuleFor(x => new { x.Id, x.Name })
            .Custom((x, context) => roleCommonValidator.RoleExistByName(x.Name, context, x.Id));

        RuleFor(x => x.Description)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "Description", context));

        RuleFor(x => x.Resources)
            .Custom((x, context) => resourceCommonValidator.ResourceExistByIdList(x, context));

        RuleFor(x => x.IsDefaultRole)
            .Custom((x, context) => roleCommonValidator.CheckIfDefaultRoleAlreadyExist(x, context));
    }
}