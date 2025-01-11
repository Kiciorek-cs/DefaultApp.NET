using Demo.BLL.Features.Auth.Commands.RevokeToken;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.BLL.Validators.Token;
using Demo.Domain.Enums.Permission;
using FluentValidation;

namespace Demo.BLL.Validators.Auth.Commands;

public class RevokeCommandValidator : CustomValidator<RevokeCommand>
{
    public RevokeCommandValidator(IPermissionUnitOfWork permissionUnitOfWork, IToken token, IClock clock)
    {
        var tokenCommonValidator = new TokenCommonValidator(token, permissionUnitOfWork, clock);

        RuleFor(x => x.AccessToken)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "AccessToken", context));

        RuleFor(x => x.AccessToken)
            .Custom((x, context) => tokenCommonValidator.TokenIsValid(x, context));

        RuleFor(x => x.AccessToken)
            .Custom((x, context) => tokenCommonValidator.TokenExist(x, TokenType.Login, context));

        RuleFor(x => x.AccessToken)
            .Custom((x, context) => tokenCommonValidator.TokenExpirationDateIsValid(x, TokenType.Login, context));
    }
}