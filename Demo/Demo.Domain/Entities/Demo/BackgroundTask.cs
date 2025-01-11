using System;
using Demo.Domain.Enums;

namespace Demo.Domain.Entities.Demo;

public class BackgroundTask
{
    public int Id { get; set; }
    public DateTimeOffset InsertedAt { get; set; }
    public BackgroundTaskStatusType StatusType { get; set; }
    public Guid ActionBlockKey { get; set; }

    public string Task { get; set; }
    public int Delay { get; set; }
}