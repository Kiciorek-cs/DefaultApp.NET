using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Permission;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.BLL.Interfaces.Services.PermissionServices;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.Domain.Enums;
using Demo.Domain.Enums.Permission;
using MediatR;

namespace Demo.BLL.Features.Auth.Commands.RefreshToken;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, IResponse>
{
    private readonly IClock _clock;
    private readonly ILogServices _logServices;
    private readonly IPermission _permission;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;
    private readonly IToken _token;

    public RefreshCommandHandler(IPermissionUnitOfWork permissionUnitOfWork, IToken token, IClock clock,
        IPermission permission, ILogServices logServices)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
        _token = token;
        _clock = clock;
        _permission = permission;
        _logServices = logServices;
    }

    public async Task<IResponse> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _permissionUnitOfWork.UserRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var refreshToken = await _permissionUnitOfWork.TokenRepository.GetAsync(x =>
                    x.Value == request.RefreshToken && x.ParentToken != null &&
                    x.ParentToken.Value == request.AccessToken &&
                    x.StatusType == TokenStatusType.Active && x.TokenType == TokenType.Refresh,
                cancellationToken);

            var login = await _permissionUnitOfWork.LoginRepository.GetAsync(x => x.Id == refreshToken.LoginId,
                cancellationToken);

            refreshToken.StatusType = refreshToken.ParentToken.StatusType = TokenStatusType.Inactive;

            var expirationDate = _clock.Current().AddMinutes(login.TokenGenerationTime);

            var jwtValue = await _token.GenerateAccessToken(login, cancellationToken);
            var jwtToken = await _permission.AddToken(login, expirationDate, jwtValue, TokenType.Login,
                cancellationToken);

            var newRefreshValue = _token.GenerateRefreshToken(cancellationToken);

            var newRefreshToken = await _permission.AddToken(login, expirationDate, newRefreshValue, TokenType.Refresh,
                cancellationToken, jwtToken.Id);

            var response = new LoginResponse
            {
                UserId = login.UserId,
                Email = login.EmailAddress,
                AccessToken = jwtValue,
                RefreshToken = newRefreshValue,
                ExpirationDate = expirationDate,
                Firstname = login.User.FirstName,
                Lastname = login.User.LastName,
                UserName = login.LoginName,
                Successful = true,
                Message = "Refresh correct"
            };

            await _logServices.AddLogToDatabase(ActionType.PUT, LogType.RefreshToken, "RefreshTokenUser", response,
                "Handle", cancellationToken, login.EmailAddress);

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