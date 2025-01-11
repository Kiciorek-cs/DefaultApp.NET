using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Helpers.Singletons;
using Demo.BLL.Interfaces.Services.EndlessMethodServices;
using Demo.BLL.Services.EndlessMethodServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demo.API.BackgroundServices;

public class StateObserver : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private bool _firstRun;

    public StateObserver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Yield();

            var queue = BackgroundServiceQueueSingleton.GetInstance();

            if (!_firstRun)
            {
                var scopeFirstRun = _serviceProvider.CreateScope();
                var scopedServiceProviderFirstRun = scopeFirstRun.ServiceProvider;


                // Downloading services with coverage area
                var doSomethingServices = scopedServiceProviderFirstRun.GetServices<IDoSomethingEndlessTask>();
                var doSomethingType = typeof(DoSomethingEndlessTask);
                var doSomethingService = doSomethingServices.First(o => o.GetType() == doSomethingType);

                // use instance od class
                await doSomethingService.ClearAndRunBackgroundJobs(cancellationToken);

                _firstRun = true;
            }


            while (cancellationToken.IsCancellationRequested == false)
                try
                {
                    await Task.Delay(10000, cancellationToken);
                    var pendingTasks = queue.GetPendingBackgroundTask();

                    var count = pendingTasks.Count;

                    for (var i = 0; i < count; i++)
                    {
                        var task = pendingTasks.ElementAtOrDefault(i);

                        var scope = _serviceProvider.CreateScope();
                        var scopedServiceProvider = scope.ServiceProvider;

                        // Downloading services with coverage area
                        var doSomethingServices = scopedServiceProvider.GetServices<IDoSomethingEndlessTask>();
                        var doSomethingType = typeof(DoSomethingEndlessTask);
                        var doSomethingService = doSomethingServices.First(o => o.GetType() == doSomethingType);

                        // use instance od class
                        await doSomethingService.ClearAndRunBackgroundJob(task.Text, task.Delay, cancellationToken);

                        queue.Remove(task);
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    Debug.WriteLine("run queue observer");
                }
                catch (OperationCanceledException)
                {
                    // execution cancelled
                }
                catch (Exception e)
                {
                    // Catch and log all exceptions,
                    // So we can continue processing other tasks
                }
        }
        catch (OperationCanceledException)
        {
            // execution cancelled
        }
        catch (Exception e)
        {
            // Catch and log all exceptions,
            // So we can continue processing other tasks
        }
    }
}