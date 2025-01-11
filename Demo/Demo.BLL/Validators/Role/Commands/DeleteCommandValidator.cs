using Demo.BLL.Features.Role.Commands.Delete;
using Demo.BLL.Interfaces.Repositories;
using FluentValidation;

namespace Demo.BLL.Validators.Role.Commands;

public class DeleteCommandValidator : CustomValidator<DeleteCommand>
{
    public DeleteCommandValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        var roleCommonValidator = new RoleCommonValidator(permissionUnitOfWork);

        RuleFor(x => x.Id)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "Id", context));

        RuleFor(x => x.Id)
            .Custom((x, context) => roleCommonValidator.RoleExistById(x, context));
    }
}