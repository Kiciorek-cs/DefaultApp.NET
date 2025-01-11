using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Services.BackgroundTaskService;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.Domain.Enums;
using MediatR;

namespace Demo.BLL.Features.BackgroundTask.Commands.DeleteAll;

public class DeleteAllCommandHandler : IRequestHandler<DeleteAllCommand, IResponse>
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDemoUnitOfWork _demoUnitOfWork;
    private readonly ILogServices _logServices;

    public DeleteAllCommandHandler(
        IDemoUnitOfWork demoUnitOfWork, ILogServices logServices, IBackgroundTaskService backgroundTaskService)
    {
        _demoUnitOfWork = demoUnitOfWork;
        _logServices = logServices;
        _backgroundTaskService = backgroundTaskService;
    }

    public async Task<IResponse> Handle(DeleteAllCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _demoUnitOfWork.BackgroundTaskRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var backgroundTasks =
                await _demoUnitOfWork.BackgroundTaskRepository.GetMultipleAsync(null, cancellationToken);

            foreach (var backgroundTask in backgroundTasks)
            {
                await _backgroundTaskService.DeleteActionBlock(backgroundTask.Task, cancellationToken);

                await _logServices.AddLogToDatabase(ActionType.DELETE, LogType.Delete, "DeleteBackgroundTask",
                    backgroundTask, "Handle", cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = 0,
                Message = "All background tasks have been deleted.",
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