using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.Domain.Enums.Permission;
using FluentValidation;

namespace Demo.BLL.Validators.Token;

public class TokenCommonValidator
{
    private readonly IClock _clock;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;
    private readonly IToken _token;

    public TokenCommonValidator(IToken token, IPermissionUnitOfWork permissionUnitOfWork, IClock clock)
    {
        _token = token;
        _permissionUnitOfWork = permissionUnitOfWork;
        _clock = clock;
    }

    public bool TokenIsValid<T>(string token, ValidationContext<T> context)
    {
        try
        {
            var (_, _) = _token.ValidToken(token);

            return true;
        }
        catch
        {
            context.AddCustomFailure("Token is not valid.", ValidationKeys.TokenIsNotValid);

            return false;
        }
    }

    public bool TokenExist<T>(string token, TokenType type, ValidationContext<T> context)
    {
        var isExist = _permissionUnitOfWork.TokenRepository
            .GetAsync(x => x.Value == token && x.StatusType == TokenStatusType.Active && x.TokenType == type)
            .GetAwaiter().GetResult() is not null;

        if (!isExist)
        {
            context.AddCustomFailure("Token is not valid.", ValidationKeys.TokenIsNotValid);

            return false;
        }

        return true;
    }

    public bool TokenExpirationDateIsValid<T>(string token, TokenType type, ValidationContext<T> context)
    {
        var tokenFromDatabase = _permissionUnitOfWork.TokenRepository
            .GetAsync(x => x.Value == token && x.StatusType == TokenStatusType.Active && x.TokenType == type)
            .GetAwaiter().GetResult();

        if (tokenFromDatabase.ExpirationDate < _clock.Current())
        {
            context.AddCustomFailure("Token expired.", ValidationKeys.TokenIsNotValid);

            return false;
        }

        return true;
    }

    public bool AccessTokenAndRefreshAreJoined<T>(string accessToken, string refreshToken, ValidationContext<T> context)
    {
        var isExist = _permissionUnitOfWork.TokenRepository.GetAsync(x =>
                x.Value == refreshToken && x.ParentToken != null && x.ParentToken.Value == accessToken &&
                x.StatusType == TokenStatusType.Active && x.TokenType == TokenType.Refresh).GetAwaiter()
            .GetResult() is not null;

        if (!isExist)
        {
            context.AddCustomFailure("The tokens are not valid.", ValidationKeys.TokenIsNotValid, "RefreshToken");

            return false;
        }

        return true;
    }
}