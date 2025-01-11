using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Helpers.Singletons;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Services.BackgroundTaskService;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.Domain.Enums;
using MediatR;

namespace Demo.BLL.Features.BackgroundTask.Commands.Create;

public class CreateCommandHandler : IRequestHandler<CreateCommand, IResponse>
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDemoUnitOfWork _demoUnitOfWork;
    private readonly ILogServices _logServices;

    public CreateCommandHandler(IDemoUnitOfWork demoUnitOfWork, ILogServices logServices,
        IBackgroundTaskService backgroundTaskService)
    {
        _demoUnitOfWork = demoUnitOfWork;
        _logServices = logServices;
        _backgroundTaskService = backgroundTaskService;
    }

    public async Task<IResponse> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _demoUnitOfWork.BackgroundTaskRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            await _backgroundTaskService.DeleteActionBlock(request.Text, cancellationToken);


            var queue = BackgroundServiceQueueSingleton.GetInstance();
            var backgroundTask = new BackgroundServiceQueueModel
            {
                Text = request.Text,
                Delay = request.Delay
            };
            queue.AddToQueue(backgroundTask);

            await _logServices.AddLogToDatabase(ActionType.POST, LogType.Create, "CreateBackgroundTask", backgroundTask,
                "Handle", cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = 0,
                Message = "The background task has been added to queue.",
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