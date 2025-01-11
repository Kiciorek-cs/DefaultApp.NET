using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Services.BackgroundTaskService;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.Domain.Enums;
using MediatR;

namespace Demo.BLL.Features.BackgroundTask.Commands.Delete;

public class DeleteCommandHandler : IRequestHandler<DeleteCommand, IResponse>
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDemoUnitOfWork _demoUnitOfWork;
    private readonly ILogServices _logServices;

    public DeleteCommandHandler(
        IDemoUnitOfWork demoUnitOfWork, ILogServices logServices, IBackgroundTaskService backgroundTaskService)
    {
        _demoUnitOfWork = demoUnitOfWork;
        _logServices = logServices;
        _backgroundTaskService = backgroundTaskService;
    }

    public async Task<IResponse> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _demoUnitOfWork.BackgroundTaskRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var backgroundTask = await _backgroundTaskService.DeleteActionBlock(request.Text, cancellationToken);

            await _logServices.AddLogToDatabase(ActionType.DELETE, LogType.Delete, "DeleteBackgroundTask",
                backgroundTask, "Handle", cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = backgroundTask.Id,
                Message = "The background task has been deleted.",
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