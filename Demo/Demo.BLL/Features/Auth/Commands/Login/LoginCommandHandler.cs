using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Helpers.Permission.Password;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Permission;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.BLL.Interfaces.Services.PermissionServices;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.BLL.Validators;
using Demo.Domain.Enums;
using Demo.Domain.Enums.Permission;
using FluentValidation;
using MediatR;

namespace Demo.BLL.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, IResponse>
{
    private readonly IClock _clock;
    private readonly ILogServices _logServices;
    private readonly IPermission _permission;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;
    private readonly IToken _token;

    public LoginCommandHandler(IPermissionUnitOfWork permissionUnitOfWork, IToken token, IClock clock,
        IPermission permission, ILogServices logServices)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
        _token = token;
        _clock = clock;
        _permission = permission;
        _logServices = logServices;
    }

    public async Task<IResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _permissionUnitOfWork.UserRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var login = await _permissionUnitOfWork.LoginRepository.GetAsync(
                x => x.EmailAddress == request.EmailAddress,
                cancellationToken);

            var sHashedPassword = PasswordHashing.HashUsingPbkdf2(request.Password, login.PasswordSalt);

            if (sHashedPassword != login.PasswordHash)
                throw new ValidationException("Invalid login or password", new List<CustomValidationFailure>
                {
                    new(null, null, "Invalid login or password", ValidationKeys.WrongLogin)
                });

            var expirationDate = _clock.Current().AddMinutes(login.TokenGenerationTime);

            var jwtValue = await _token.GenerateAccessToken(login, cancellationToken);
            
            var jwtToken = await _permission.AddToken(login, expirationDate, jwtValue, TokenType.Login,
                cancellationToken);

            var refreshValue = _token.GenerateRefreshToken(cancellationToken);
            
            var newRefreshToken = await _permission.AddToken(login, expirationDate, refreshValue, TokenType.Refresh,
                cancellationToken, jwtToken.Id);

            var response = new LoginResponse
            {
                UserId = login.UserId,
                Email = login.EmailAddress,
                AccessToken = jwtValue,
                RefreshToken = refreshValue,
                ExpirationDate = expirationDate,
                Firstname = login.User.FirstName,
                Lastname = login.User.LastName,
                UserName = login.LoginName,
                Successful = true,
                Message = "Login successful"
            };

            await _logServices.AddLogToDatabase(ActionType.POST, LogType.Login, "LoginUser", response, "Handle",
                cancellationToken, login.EmailAddress);

            await transaction.CommitAsync(cancellationToken);

            return response;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}