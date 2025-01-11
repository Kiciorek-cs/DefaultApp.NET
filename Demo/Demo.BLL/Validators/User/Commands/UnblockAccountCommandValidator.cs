using Demo.BLL.Features.User.Commands.UnblockAccount;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Enums.Permission;
using FluentValidation;

namespace Demo.BLL.Validators.User.Commands;

public class UnblockAccountCommandValidator : CustomValidator<UnblockAccountCommand>
{
    public UnblockAccountCommandValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        var userCommonValidator = new UserCommonValidator(permissionUnitOfWork);

        RuleFor(x => x.Id)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "Id", context));

        RuleFor(x => x.Id)
            .Custom((x, context) => userCommonValidator.UserExistById(x, context));

        RuleFor(x => x.Id)
            .Custom((x, context) =>
                userCommonValidator.CheckIfUserIsInCurrentState(x, AccountStatusType.Active, context));
    }
}