using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.Domain.Enums;
using MediatR;

namespace Demo.BLL.Features.Resource.Commands.Delete;

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
        var transaction = await _permissionUnitOfWork.ResourceRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var resource =
                await _permissionUnitOfWork.ResourceRepository.GetAsync(x => x.Id == request.Id, cancellationToken);

            await _logServices.AddLogToDatabase(ActionType.DELETE, LogType.Delete, "DeletePermissionResource", resource,
                "Handle", cancellationToken);

            _permissionUnitOfWork.ResourceRepository.Delete(resource);

            await _permissionUnitOfWork.CommitAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = resource.Id,
                Message = "The resource has been deleted.",
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