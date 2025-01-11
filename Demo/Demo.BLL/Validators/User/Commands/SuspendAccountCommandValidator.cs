using Demo.BLL.Features.User.Commands.SuspendAccount;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Enums.Permission;
using FluentValidation;

namespace Demo.BLL.Validators.User.Commands;

public class SuspendAccountCommandValidator : CustomValidator<SuspendAccountCommand>
{
    public SuspendAccountCommandValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        var userCommonValidator = new UserCommonValidator(permissionUnitOfWork);

        RuleFor(x => x.Id)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "Id", context));

        RuleFor(x => x.Id)
            .Custom((x, context) => userCommonValidator.UserExistById(x, context));

        RuleFor(x => x.Id)
            .Custom((x, context) =>
                userCommonValidator.CheckIfUserIsInCurrentState(x, AccountStatusType.Suspended, context));
    }
}