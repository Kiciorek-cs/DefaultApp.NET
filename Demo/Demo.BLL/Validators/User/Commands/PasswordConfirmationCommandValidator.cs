using Demo.BLL.Features.User.Commands.PasswordConfirmation;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.BLL.Validators.Token;
using Demo.Domain.Enums.Permission;
using FluentValidation;

namespace Demo.BLL.Validators.User.Commands;

public class PasswordConfirmationCommandValidator : CustomValidator<PasswordConfirmationCommand>
{
    public PasswordConfirmationCommandValidator(IPermissionUnitOfWork permissionUnitOfWork, IToken token, IClock clock)
    {
        var tokenCommonValidator = new TokenCommonValidator(token, permissionUnitOfWork, clock);

        RuleFor(x => x.ResetToken)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "ConfirmationToken", context));

        RuleFor(x => x.ResetToken)
            .Custom((x, context) => tokenCommonValidator.TokenIsValid(x, context));

        RuleFor(x => x.ResetToken)
            .Custom((x, context) => tokenCommonValidator.TokenExist(x, TokenType.Reset, context));

        RuleFor(x => x.ResetToken)
            .Custom((x, context) => tokenCommonValidator.TokenExpirationDateIsValid(x, TokenType.Reset, context));

        RuleFor(x => x.Password)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "Password", context));

        RuleFor(x => x.Password)
            .Custom((x, context) => CommonValidator.PasswordChecker(x, context));

        RuleFor(x => x.ConfirmPassword)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "ConfirmPassword", context));

        RuleFor(x => new { x.Password, x.ConfirmPassword })
            .Custom((x, context) =>
                CommonValidator.PasswordAndConfirmPasswordAreTheSame(x.Password, x.ConfirmPassword, context));
    }
}