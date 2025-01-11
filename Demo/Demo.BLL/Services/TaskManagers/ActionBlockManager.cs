using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Demo.BLL.Interfaces.Services.TaskManagers;

namespace Demo.BLL.Services.TaskManagers;

public class ActionBlockManager : IActionBlockManager
{
    public (ActionBlock<T>, CancellationTokenSource) CreateActionBlock<T>(Func<T, Task> function, SemaphoreSlim @lock,
        int daley, bool isReadAction = false)
    {
        var token = new CancellationTokenSource();

        var actionBlock = CreateGenericNeverEndingTask(function, @lock, daley, token.Token);

        return (actionBlock, token);
    }

    public (List<ActionBlock<T>>, List<CancellationTokenSource>) CreateActionBlockList<T>(Func<T, Task> function,
            List<SemaphoreSlim> @lock, int actionBlockCount, List<int> daleyList = null, bool isReadAction = false)
    {
        var tokens = new List<CancellationTokenSource>();
        var actionBlockList = new List<ActionBlock<T>>();

        if (daleyList is null)
        {
            daleyList = new List<int>();
            daleyList.AddRange(Enumerable.Repeat(1, actionBlockCount));
        }

        for (var i = 0; i < actionBlockCount; i++)
        {
            var delay = daleyList.ElementAtOrDefault(i);
            var lockObject = @lock.ElementAtOrDefault(i);

            var token = new CancellationTokenSource();

            actionBlockList.Add(CreateGenericNeverEndingTask(function, lockObject, delay, token.Token));
            tokens.Add(token);
        }

        return (actionBlockList, tokens);
    }

    public (ActionBlock<T>, CancellationTokenSource) CreateActionBlock<T>(Func<T, Task> function,
        ReaderWriterLockSlim @lock, int daley, bool isReadAction = false)
    {
        var token = new CancellationTokenSource();

        var actionBlock = CreateGenericNeverEndingTask(function, @lock, daley, isReadAction, token.Token);

        return (actionBlock, token);
    }

    public (List<ActionBlock<T>>, CancellationTokenSource) CreateActionBlockList<T>(Func<T, Task> function,
            List<ReaderWriterLockSlim> @lock, int actionBlockCount, List<int> daleyList = null,
            bool isReadAction = false)
    {
        var token = new CancellationTokenSource();
        var actionBlockList = new List<ActionBlock<T>>();

        if (daleyList is null)
        {
            daleyList = new List<int>();
            daleyList.AddRange(Enumerable.Repeat(1, actionBlockCount));
        }

        for (var i = 0; i < actionBlockCount; i++)
        {
            var delay = daleyList.ElementAtOrDefault(i);
            var lockObject = @lock.ElementAtOrDefault(i);

            actionBlockList.Add(CreateGenericNeverEndingTask(function, lockObject, delay, isReadAction, token.Token));
        }

        return (actionBlockList, token);
    }

    private ActionBlock<T> CreateGenericNeverEndingTask<T>(Func<T, Task> action, SemaphoreSlim _lock, int delay, CancellationToken cancellationToken)
    {
        if (action == null) throw new ArgumentNullException("Action is not define.");

        // Declare the block variable, it needs to be captured.
        ActionBlock<T> block = null;

        // Create the block, it will call itself, so
        // you need to separate the declaration and the assignment.
        // Async so you can wait easily when the delay comes.
        block = new ActionBlock<T>(async genericVariable =>
        {
            await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken).
                // Same as above.
                ConfigureAwait(false);

            //if (isReadAction)
            //    _lock.EnterReadLock();
            //else
            //    _lock.EnterWriteLock();
            await _lock.WaitAsync(cancellationToken);
            try
            {
                //await Task.Delay(TimeSpan.FromSeconds(20)).
                //    // Same as above.
                //    ConfigureAwait(false);
                //if (_lock.IsWriteLockHeld)
                //    block?.Post(genericVariable);

                // Perform the action.  Wait on the result.
                await action(genericVariable) //.
                    // Doing this here because synchronization context more than
                    // likely *doesn't* need to be captured for the continuation
                    // here.  As a matter of fact, that would be downright dangerous.
                    .ConfigureAwait(false);
            }
            finally
            {
                ////_lock.ExitReadLock();
                //if (isReadAction && _lock.IsReadLockHeld)
                //    _lock.ExitReadLock();
                //else if(_lock.IsWriteLockHeld)
                //    _lock.ExitWriteLock();

                _lock.Release();
            }
            
            // Post the action back to the block.
            //await block?.SendAsync(genericVariable, cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
                block?.Post(genericVariable);
        }, new ExecutionDataflowBlockOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = 1
        });

        return block;
    }

    private ActionBlock<T> CreateGenericNeverEndingTask<T>(Func<T, Task> action, ReaderWriterLockSlim _lock, int delay,
        bool isReadAction, CancellationToken cancellationToken)
    {
        if (action == null) throw new ArgumentNullException("Action is not define.");

        // Declare the block variable, it needs to be captured.
        ActionBlock<T> block = null;

        // Create the block, it will call itself, so
        // you need to separate the declaration and the assignment.
        // Async so you can wait easily when the delay comes.
        block = new ActionBlock<T>(async genericVariable =>
        {
            try
            {
                if (isReadAction)
                    _lock.EnterReadLock();
                else
                    _lock.EnterWriteLock();
                //await _lock.WaitAsync(cancellationToken);
                try
                {
                    //if (_lock.IsWriteLockHeld)
                    //    block?.Post(genericVariable);

                    // Perform the action.  Wait on the result.
                    await action(genericVariable); //.
                    // Doing this here because synchronization context more than
                    // likely *doesn't* need to be captured for the continuation
                    // here.  As a matter of fact, that would be downright dangerous.
                    //ConfigureAwait(false);
                }
                finally
                {
                    //zwolnij blokadę do odczytu

                    //_lock.ExitReadLock();
                    if (isReadAction && _lock.IsReadLockHeld)
                        _lock.ExitReadLock();
                    else if (_lock.IsWriteLockHeld)
                        _lock.ExitWriteLock();

                    //_lock.Release();
                }
            }
            catch (Exception ex)
            {
            }
            // Wait.

            //var tt = TimeSpan.FromSeconds(delay);

            await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken).
                // Same as above.
                ConfigureAwait(false);

            // Post the action back to the block.
            //await block?.SendAsync(genericVariable, cancellationToken);
            block?.Post(genericVariable);
        }, new ExecutionDataflowBlockOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = 1
        });

        return block;
    }
}