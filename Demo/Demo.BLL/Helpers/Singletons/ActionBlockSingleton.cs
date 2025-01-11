using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace Demo.BLL.Helpers.Singletons;

// Singleton holding shares
public class ActionBlockSingleton<T>
{
    private static ActionBlockSingleton<T> instance;
    private readonly Dictionary<Guid, ActionBlockModel<T>> actionBlocks;

    private ActionBlockSingleton()
    {
        actionBlocks = new Dictionary<Guid, ActionBlockModel<T>>();
    }

    public static ActionBlockSingleton<T> GetInstance()
    {
        if (instance == null) instance = new ActionBlockSingleton<T>();
        return instance;
    }

    public void AddActionBlock(Guid key, ActionBlockModel<T> actionBlock)
    {
        actionBlocks[key] = actionBlock;
    }

    public ActionBlockModel<T> GetActionBlock(Guid key)
    {
        if (actionBlocks.TryGetValue(key, out var block)) return block;
        return null;
    }

    public List<Guid> GetAllKeys()
    {
        return actionBlocks.Select(x => x.Key).ToList();
    }

    public List<ActionBlockModel<T>> GetAllActionBlock()
    {
        return actionBlocks.Select(x => x.Value).ToList();
    }

    public ActionBlockModel<T> GetByText(string text)
    {
        return actionBlocks.Values.FirstOrDefault(x => x.Text == text);
    }

    public void RemoveActionBlock(Guid key)
    {
        actionBlocks.Remove(key);
    }
}

public class ActionBlockModel<T>
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public int Delay { get; set; }
    public ActionBlock<T> ActionBlockManager { get; set; }
    public CancellationTokenSource CancellationTokenSource { get; set; }
}

public class BackgroundServiceQueueSingleton
{
    private static BackgroundServiceQueueSingleton _instance;
    private List<BackgroundServiceQueueModel> _pendingQueue;
    private List<BackgroundServiceQueueModel> _workingQueue;

    private BackgroundServiceQueueSingleton()
    {
        _pendingQueue = new List<BackgroundServiceQueueModel>();
        _workingQueue = new List<BackgroundServiceQueueModel>();
    }

    public static BackgroundServiceQueueSingleton GetInstance()
    {
        if (_instance == null) _instance = new BackgroundServiceQueueSingleton();
        return _instance;
    }

    public void AddToQueue(BackgroundServiceQueueModel backgroundServiceQueueModel)
    {
        _pendingQueue.Add(backgroundServiceQueueModel);
    }

    public void AddToQueue(List<BackgroundServiceQueueModel> backgroundsServiceQueueModel)
    {
        _pendingQueue.AddRange(backgroundsServiceQueueModel);
    }

    public void MoveToWorking(BackgroundServiceQueueModel backgroundServiceQueueModel)
    {
        _workingQueue.Add(backgroundServiceQueueModel);
        _pendingQueue.Remove(backgroundServiceQueueModel);
    }

    public void RemoveByText(string text)
    {
        _pendingQueue = _workingQueue.Where(x => x != null).ToList();
        _workingQueue = _workingQueue.Where(x => x != null).ToList();

        _pendingQueue = _pendingQueue.Where(x =>
            x.Text != text).ToList();

        _workingQueue = _workingQueue.Where(x =>
            x.Text != text).ToList();
    }

    public List<BackgroundServiceQueueModel> GetPendingBackgroundTask()
    {
        return _pendingQueue;
    }

    public void Remove(BackgroundServiceQueueModel backgroundServiceQueueModel)
    {
        _pendingQueue.Remove(backgroundServiceQueueModel);
        _workingQueue.Add(backgroundServiceQueueModel);
    }

    public BackgroundServiceQueueModel RunBackgroundTask()
    {
        if (_pendingQueue.Any())
        {
            var first = _pendingQueue.First();
            _workingQueue.Add(first);
            _pendingQueue.Remove(first);
            return first;
        }

        return null;
    }
}

public class BackgroundServiceQueueModel
{
    public string Text { get; set; }
    public int Delay { get; set; }
}