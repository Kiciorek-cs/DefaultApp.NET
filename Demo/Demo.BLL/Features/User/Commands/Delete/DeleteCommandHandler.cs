using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.Domain.Enums;
using Demo.Domain.Enums.Permission;
using MediatR;

namespace Demo.BLL.Features.User.Commands.Delete;

public class DeleteCommandHandler : IRequestHandler<DeleteCommand, IResponse>
{
    private readonly ILogServices _logServices;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public DeleteCommandHandler(IPermissionUnitOfWork permissionUnitOfWork, ILogServices logServices)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
        _logServices = logServices;
    }

    public async Task<IResponse> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _permissionUnitOfWork.UserRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = await _permissionUnitOfWork.UserRepository.GetAsync(x => x.Id == request.Id, cancellationToken);

            user.Login.Status = AccountStatusType.Deleted;

            var tokens = user.Login.Tokens.Where(x => x.StatusType == TokenStatusType.Active);
            foreach (var token in tokens) token.StatusType = TokenStatusType.Inactive;

            await _logServices.AddLogToDatabase(ActionType.DELETE, LogType.Delete, "DeleteUser", user, "Handle",
                cancellationToken, user.Login.EmailAddress);

            await _permissionUnitOfWork.CommitAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = user.Id,
                Message = "User has been deleted.",
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