using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.Domain.Enums;
using Demo.Domain.Enums.Permission;
using MediatR;

namespace Demo.BLL.Features.User.Commands.ConfirmAccountByUserId;

public class ConfirmAccountByUserIdCommandHandler : IRequestHandler<ConfirmAccountByUserIdCommand, IResponse>
{
    private readonly ILogServices _logServices;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;
    private readonly IToken _token;

    public ConfirmAccountByUserIdCommandHandler(IPermissionUnitOfWork permissionUnitOfWork, IToken token,
        ILogServices logServices)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
        _token = token;
        _logServices = logServices;
    }

    public async Task<IResponse> Handle(ConfirmAccountByUserIdCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _permissionUnitOfWork.UserRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var userToken = _token.GetTokenInformationFromHeader(cancellationToken);

            var user = await _permissionUnitOfWork.UserRepository.GetAsync(x => x.Id == request.UserId,
                cancellationToken);

            var userTokens = user.Login.Tokens.Where(x =>
                x.StatusType == TokenStatusType.Active);
            foreach (var token in userTokens) token.StatusType = TokenStatusType.Inactive;

            user.Login.Status = AccountStatusType.Active;
            user.Login.EmailValidationStatus = EmailValidationStatusType.Confirmed;

            await _permissionUnitOfWork.CommitAsync(cancellationToken);

            await _logServices.AddLogToDatabase(ActionType.PUT, LogType.Update, "ConfirmAccountByUserId", user,
                "Handle", cancellationToken, userToken.user.EmailAddress);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = user.Id,
                Message = "Account has been activated.",
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