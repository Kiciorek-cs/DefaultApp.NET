using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Demo.Domain.Entities.Demo;

namespace Demo.BLL.Interfaces.Services.BackgroundTaskService;

public interface IBackgroundTaskService
{
    Task AddBackgroundTaskInformation<T>(
        Guid guid,
        string text,
        int delay,
        ActionBlock<T> actionBlock,
        CancellationToken cancellationToken,
        CancellationTokenSource cancellationTokenSource);

    Task<BackgroundTask> DeleteActionBlock(string text, CancellationToken cancellationToken);
}