using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.Domain.Enums;
using Demo.Domain.Enums.Permission;
using MediatR;

namespace Demo.BLL.Features.User.Commands.UnblockAccount;

public class UnblockAccountCommandHandler : IRequestHandler<UnblockAccountCommand, IResponse>
{
    private readonly ILogServices _logServices;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public UnblockAccountCommandHandler(IPermissionUnitOfWork permissionUnitOfWork, ILogServices logServices)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
        _logServices = logServices;
    }

    public async Task<IResponse> Handle(UnblockAccountCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _permissionUnitOfWork.UserRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = await _permissionUnitOfWork.UserRepository.GetAsync(x => x.Id == request.Id, cancellationToken);

            user.Login.Status = AccountStatusType.Active;

            await _permissionUnitOfWork.CommitAsync(cancellationToken);

            await _logServices.AddLogToDatabase(ActionType.PUT, LogType.Update, "UnblockAccount", user, "Handle",
                cancellationToken, user.Login.EmailAddress);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = user.Id,
                Message = "User has been unblocked.",
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