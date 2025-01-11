using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Demo.BLL.Interfaces.Services.TaskManagers;

public interface IActionBlockManager
{
    /// <summary>
    ///     Create one action block
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="function"></param>
    /// <param name="daley">In second</param>
    /// <returns></returns>
    (ActionBlock<T>, CancellationTokenSource) CreateActionBlock<T>(Func<T, Task> function, SemaphoreSlim @lock,
        int daley = 1, bool isReadAction = false);

    /// <summary>
    ///     Create one action block
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="function"></param>
    /// <param name="lock"></param>
    /// <param name="daley">In second</param>
    /// <param name="isReadAction"></param>
    /// <returns></returns>
    (ActionBlock<T>, CancellationTokenSource) CreateActionBlock<T>(Func<T, Task> function, ReaderWriterLockSlim @lock,
        int daley = 1, bool isReadAction = false);

    /// <summary>
    ///     Create list of action block
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="function"></param>
    /// <param name="lock"></param>
    /// <param name="actionBlockCount"></param>
    /// <param name="daleyList">Default value is 1</param>
    /// <param name="isReadAction"></param>
    /// <returns></returns>
    (List<ActionBlock<T>>, List<CancellationTokenSource>) CreateActionBlockList<T>(Func<T, Task> function,
        List<SemaphoreSlim> @lock,
        int actionBlockCount = 1, List<int> daleyList = null, bool isReadAction = false);

    /// <summary>
    ///     Create list of action block
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="function"></param>
    /// <param name="lock"></param>
    /// <param name="actionBlockCount"></param>
    /// <param name="daleyList">Default value is 1</param>
    /// <param name="isReadAction"></param>
    /// <returns></returns>
    (List<ActionBlock<T>>, CancellationTokenSource) CreateActionBlockList<T>(Func<T, Task> function,
        List<ReaderWriterLockSlim> @lock,
        int actionBlockCount = 1, List<int> daleyList = null, bool isReadAction = false);
}