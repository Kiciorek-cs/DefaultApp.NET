using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.Domain.Enums;
using Demo.Domain.Enums.Permission;
using MediatR;

namespace Demo.BLL.Features.Auth.Commands.RevokeToken;

public class RevokeCommandHandler : IRequestHandler<RevokeCommand, IResponse>
{
    private readonly ILogServices _logServices;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public RevokeCommandHandler(IPermissionUnitOfWork permissionUnitOfWork, ILogServices logServices)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
        _logServices = logServices;
    }

    public async Task<IResponse> Handle(RevokeCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _permissionUnitOfWork.UserRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var token = await _permissionUnitOfWork.TokenRepository.GetAsync(
                x => x.Value == request.AccessToken && x.StatusType == TokenStatusType.Active &&
                     x.TokenType == TokenType.Login,
                cancellationToken);

            token.StatusType = TokenStatusType.Inactive;
            foreach (var subToken in token.SubTokens) subToken.StatusType = TokenStatusType.Inactive;
            await _permissionUnitOfWork.CommitAsync(cancellationToken);

            await _logServices.AddLogToDatabase(ActionType.DELETE, LogType.RevokeToken, "RevokeTokenUser",
                token.Login.User, "Handle", cancellationToken, token.Login.EmailAddress);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = token.Login.UserId,
                Message = "Tokens have been deactivated.",
                Successful = true
            };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}