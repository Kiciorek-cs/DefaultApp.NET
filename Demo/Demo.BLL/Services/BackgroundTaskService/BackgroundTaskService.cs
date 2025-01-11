using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Demo.BLL.Helpers.Singletons;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Services.BackgroundTaskService;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.Domain.Entities.Demo;
using Demo.Domain.Enums;

namespace Demo.BLL.Services.BackgroundTaskService;

public class BackgroundTaskService : IBackgroundTaskService
{
    private readonly IClock _clock;
    private readonly IDemoUnitOfWork _demoUnitOfWork;

    public BackgroundTaskService(IDemoUnitOfWork demoUnitOfWork, IClock clock)
    {
        _demoUnitOfWork = demoUnitOfWork;
        _clock = clock;
    }

    public async Task AddBackgroundTaskInformation<T>(
        Guid guid,
        string text,
        int delay,
        ActionBlock<T> actionBlock,
        CancellationToken cancellationToken,
        CancellationTokenSource cancellationTokenSource)
    {
        var singleton = ActionBlockSingleton<T>.GetInstance();

        singleton.AddActionBlock(guid, new ActionBlockModel<T>
        {
            Id = guid,
            Text = text,
            Delay = delay,
            ActionBlockManager = actionBlock,
            CancellationTokenSource = cancellationTokenSource
        });

        await _demoUnitOfWork.BackgroundTaskRepository.AddAsync(new BackgroundTask
        {
            Task = text,
            Delay = delay,
            ActionBlockKey = guid,
            InsertedAt = _clock.Current(),
            StatusType = BackgroundTaskStatusType.Active
        }, cancellationToken);

        await _demoUnitOfWork.CommitAsync(cancellationToken);
    }

    public async Task<BackgroundTask> DeleteActionBlock(string text, CancellationToken cancellationToken)
    {
        var singleton = ActionBlockSingleton<Tuple<Guid, CancellationToken, SemaphoreSlim, string>>.GetInstance();

        var actionBlock = singleton.GetByText(text);

        if (actionBlock is not null)
        {
            //can't be await because it will create a loop
            Task.Run(() => { WaitHandle.WaitAny(new[] { actionBlock.CancellationTokenSource.Token.WaitHandle }); },
                cancellationToken);
            await actionBlock.CancellationTokenSource.CancelAsync();

            singleton.RemoveActionBlock(actionBlock.Id);
        }

        var backgroundTask = await _demoUnitOfWork.BackgroundTaskRepository.GetAsync(x =>
            x.Task == text, cancellationToken);

        if (backgroundTask is not null)
        {
            _demoUnitOfWork.BackgroundTaskRepository.Delete(backgroundTask);
            await _demoUnitOfWork.CommitAsync(cancellationToken);
        }

        var queue = BackgroundServiceQueueSingleton.GetInstance();
        queue.RemoveByText(text);

        return backgroundTask;
    }
}