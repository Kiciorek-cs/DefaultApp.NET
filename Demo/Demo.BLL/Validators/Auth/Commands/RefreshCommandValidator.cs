using Demo.BLL.Features.Auth.Commands.RefreshToken;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.BLL.Validators.Token;
using Demo.Domain.Enums.Permission;
using FluentValidation;

namespace Demo.BLL.Validators.Auth.Commands;

public class RefreshCommandValidator : CustomValidator<RefreshCommand>
{
    public RefreshCommandValidator(IPermissionUnitOfWork permissionUnitOfWork, IToken token, IClock clock)
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

        RuleFor(x => x.RefreshToken)
            .Custom((x, context) => CommonValidator.WhetherTheStringValueIsValid(x, "RefreshToken", context));

        RuleFor(x => x.RefreshToken)
            .Custom((x, context) => tokenCommonValidator.TokenExist(x, TokenType.Refresh, context));

        RuleFor(x => x.RefreshToken)
            .Custom((x, context) => tokenCommonValidator.TokenExpirationDateIsValid(x, TokenType.Refresh, context));

        RuleFor(x => new { x.AccessToken, x.RefreshToken })
            .Custom((x, context) =>
                tokenCommonValidator.AccessTokenAndRefreshAreJoined(x.AccessToken, x.RefreshToken, context));
    }
}