using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Services.ErrorLogger;
using Demo.Domain.Enums;
using MediatR;
using Serilog;

namespace Demo.BLL.Features.EndlessTask.Publishes.DoSomething;

public class DoSomethingNotificationHandler : INotificationHandler<DoSomethingNotification>
{
    private readonly IClock _clock;
    private readonly IDemoUnitOfWork _demoUnitOfWork;

    public DoSomethingNotificationHandler(
        IDemoUnitOfWork demoUnitOfWork, IClock clock)
    {
        _demoUnitOfWork = demoUnitOfWork;
        _clock = clock;
    }

    public async Task Handle(DoSomethingNotification request, CancellationToken cancellationToken)
    {
        //var _lock = request.SemaphoreSlim;
        var payload = request.Text;
        var guid = request.Guid;
        try
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Debug.WriteLine(
                $"Background job is doing some operation {guid} ----- {Environment.CurrentManagedThreadId} - {payload}");

            var chars = payload.ToCharArray();

            foreach (var c in chars)
            {
                await Task.Delay(2000);
                Debug.WriteLine($"{guid} ----- {c}");
            }

            stopwatch.Stop();
            var seconds = stopwatch.Elapsed;

            Debug.WriteLine(
                $"Background job finished operation: {guid} ----- {Environment.CurrentManagedThreadId} ----- {seconds} seconds");
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine("Operation was cancaled");

            Log.Information("Operation was cancaled.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception: {ex.Message}");

            Log.Error($"Exception: {ex.Message}.");

            ErrorLogger.LogError(
                _clock.Current(),
                $"-----Exception error: {ex.Message} \n");
        }
        finally
        {
            await CreateLogOnDatabase(guid, payload, cancellationToken);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    private async Task CreateLogOnDatabase(Guid guid, string payload, CancellationToken cancellationToken = default)
    {
        var newLog = new Domain.Entities.Demo.Log
        {
            InsertedBy = "Background job",
            InsertedOn = _clock.Current(),
            LogType = LogType.Information,
            ActionType = ActionType.UNDEFINED,
            Description = $"Background task has finished working. {payload}",
            Name = "The word was spelled",
            MethodName = "Background job",
            TraceId = guid.ToString()
        };

        await _demoUnitOfWork.LogDapperRepository.InsertLog(newLog, cancellationToken);

        Log.Information($"Log has been added {guid}.");
    }
}