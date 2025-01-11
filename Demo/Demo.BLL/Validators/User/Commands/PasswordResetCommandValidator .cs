using Demo.BLL.Features.User.Commands.PasswordReset;
using Demo.BLL.Interfaces.Repositories;
using FluentValidation;

namespace Demo.BLL.Validators.User.Commands;

public class PasswordResetCommandValidator : CustomValidator<PasswordResetCommand>
{
    public PasswordResetCommandValidator(IPermissionUnitOfWork permissionUnitOfWork)
    {
        var userCommonValidator = new UserCommonValidator(permissionUnitOfWork);

        RuleFor(x => x.EmailAddress)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "EmailAddress", context));

        RuleFor(x => x.EmailAddress)
            .Custom((x, context) => userCommonValidator.UserExistByEmailAddress(x, context));

        RuleFor(x => x.EmailAddress)
            .Custom((x, context) => userCommonValidator.WhetherUserHaveActiveAccount(x, context));
    }
}