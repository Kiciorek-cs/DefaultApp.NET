using Demo.BLL.Features.User.Commands.ConfirmAccount;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.BLL.Validators.Token;
using Demo.Domain.Enums.Permission;
using FluentValidation;

namespace Demo.BLL.Validators.User.Commands;

public class ConfirmAccountCommandValidator : CustomValidator<ConfirmAccountCommand>
{
    public ConfirmAccountCommandValidator(IPermissionUnitOfWork permissionUnitOfWork, IToken token, IClock clock)
    {
        var userCommonValidator = new UserCommonValidator(permissionUnitOfWork);
        var tokenCommonValidator = new TokenCommonValidator(token, permissionUnitOfWork, clock);

        RuleFor(x => x.ConfirmationToken)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "ConfirmationToken", context));

        RuleFor(x => x.ConfirmationToken)
            .Custom((x, context) => tokenCommonValidator.TokenIsValid(x, context));

        RuleFor(x => x.ConfirmationToken)
            .Custom((x, context) => tokenCommonValidator.TokenExist(x, TokenType.Confirmation, context));

        RuleFor(x => x.ConfirmationToken)
            .Custom((x, context) =>
                tokenCommonValidator.TokenExpirationDateIsValid(x, TokenType.Confirmation, context));

        RuleFor(x => x.UserId)
            .Custom((x, context) => CommonValidator.WhetherTheIdIsCorrect(x, "UserId", context));

        RuleFor(x => x.UserId)
            .Custom((x, context) => userCommonValidator.UserExistById(x, context));

        RuleFor(x => x.UserId)
            .Custom((x, context) => userCommonValidator.UserCanBeConfirmed(x, context));
    }
}