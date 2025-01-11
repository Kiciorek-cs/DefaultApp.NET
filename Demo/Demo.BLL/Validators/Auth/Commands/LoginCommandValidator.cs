using Demo.BLL.Features.Auth.Commands.Login;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Validators.User;
using FluentValidation;

namespace Demo.BLL.Validators.Auth.Commands;

public class LoginCommandValidator : CustomValidator<LoginCommand>
{
    public LoginCommandValidator(IPermissionUnitOfWork permissionUnitOfWork)
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