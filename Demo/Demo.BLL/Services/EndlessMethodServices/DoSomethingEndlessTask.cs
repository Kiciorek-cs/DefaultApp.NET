using System;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Features.EndlessTask.Publishes.DoSomething;
using Demo.BLL.Helpers.Singletons;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Services.BackgroundTaskService;
using Demo.BLL.Interfaces.Services.EndlessMethodServices;
using Demo.BLL.Interfaces.Services.TaskManagers;
using Demo.Domain.Enums;
using MediatR;

namespace Demo.BLL.Services.EndlessMethodServices;

public class DoSomethingEndlessTask : IDoSomethingEndlessTask
{
    private readonly IActionBlockManager _actionBlockManager;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDemoUnitOfWork _demoUnitOfWork;
    private readonly IMediator _mediator;

    public DoSomethingEndlessTask(IDemoUnitOfWork demoUnitOfWork, IActionBlockManager actionBlockManager,
        IMediator mediator, IBackgroundTaskService backgroundTaskService)
    {
        _mediator = mediator;
        _backgroundTaskService = backgroundTaskService;
        _demoUnitOfWork = demoUnitOfWork;
        _actionBlockManager = actionBlockManager;
    }


    public async Task ClearAndRunBackgroundJob(string text, int daley, CancellationToken cancellationToken)
    {
        var backgroundTask =
            await _demoUnitOfWork.BackgroundTaskRepository.GetAsync(x =>
                x.Task == text, cancellationToken);

        var singletonJob = ActionBlockSingleton<Tuple<Guid, CancellationToken, SemaphoreSlim, string>>.GetInstance();

        var singletonTask = singletonJob.GetByText(text);

        if (backgroundTask != null) _demoUnitOfWork.BackgroundTaskRepository.Delete(backgroundTask);

        if (singletonTask != null)
        {
            await singletonTask.CancellationTokenSource.CancelAsync();

            singletonJob.RemoveActionBlock(singletonTask.Id);
        }

        await RunBackgroundJob(text, daley, cancellationToken);
    }

    public async Task ClearAndRunBackgroundJobs(CancellationToken cancellationToken)
    {
        var backgroundTasks =
            await _demoUnitOfWork.BackgroundTaskRepository.GetMultipleAsync(
                x => x.StatusType == BackgroundTaskStatusType.Active, cancellationToken);

        foreach (var backgroundTask in backgroundTasks)
            await ClearAndRunBackgroundJob(backgroundTask.Task, backgroundTask.Delay, cancellationToken);
    }

    private async Task RunBackgroundJob(string text, int daley, CancellationToken cancellationToken)
    {
        var semaphore = new SemaphoreSlim(1, 1);

        var (actionBlock, cancellationTokenSource) =
            _actionBlockManager
                .CreateActionBlock<Tuple<Guid, CancellationToken, SemaphoreSlim, string>>(
                    PublishChecker,
                    semaphore,
                    daley,
                    true);

        var guid = Guid.NewGuid();

        await _backgroundTaskService.AddBackgroundTaskInformation(guid, text, daley, actionBlock, cancellationToken,
            cancellationTokenSource);

        actionBlock.Post(
            new Tuple<Guid, CancellationToken, SemaphoreSlim, string>(
                guid, cancellationTokenSource.Token, semaphore, text
            )
        );

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    private async Task PublishChecker(Tuple<Guid, CancellationToken, SemaphoreSlim, string> tuple)
    {
        var (guid, token, semaphoreSlim, text) = tuple;

        await _mediator.Publish(new DoSomethingNotification
        {
            Guid = guid,
            Text = text,
            SemaphoreSlim = semaphoreSlim
        }, token);
    }
}

public class SubmitData
{
    public SubmitData(
        Guid guid,
        string text)
    {
        Guid = guid;
        Text = text;
    }

    private Guid Guid { get; }
    private string Text { get; }

    internal Guid GetGuid()
    {
        return Guid;
    }

    internal string GetText()
    {
        return Text;
    }
}