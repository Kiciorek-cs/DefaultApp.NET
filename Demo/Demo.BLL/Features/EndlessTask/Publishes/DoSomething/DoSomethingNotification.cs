using System;
using System.Threading;
using MediatR;

namespace Demo.BLL.Features.EndlessTask.Publishes.DoSomething;

public class DoSomethingNotification : INotification
{
    public Guid Guid { get; set; }
    public string Text { get; set; }
    public SemaphoreSlim SemaphoreSlim { get; set; }
}