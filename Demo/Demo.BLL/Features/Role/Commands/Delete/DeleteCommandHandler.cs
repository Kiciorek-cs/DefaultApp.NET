using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.Domain.Enums;
using MediatR;

namespace Demo.BLL.Features.Role.Commands.Delete;

public class DeleteCommandHandler : IRequestHandler<DeleteCommand, IResponse>
{
    private readonly ILogServices _logServices;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public DeleteCommandHandler(
        ILogServices logServices, IPermissionUnitOfWork permissionUnitOfWork)
    {
        _logServices = logServices;
        _permissionUnitOfWork = permissionUnitOfWork;
    }

    public async Task<IResponse> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _permissionUnitOfWork.RoleRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var role =
                await _permissionUnitOfWork.RoleRepository.GetAsync(x => x.Id == request.Id, cancellationToken);

            await _logServices.AddLogToDatabase(ActionType.DELETE, LogType.Delete, "DeletePermissionRole", role,
                "Handle", cancellationToken);

            _permissionUnitOfWork.RoleRepository.Delete(role);

            await _permissionUnitOfWork.CommitAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = role.Id,
                Message = "The role has been deleted.",
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