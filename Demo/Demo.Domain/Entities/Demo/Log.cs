using System;
using Demo.Domain.Enums;

namespace Demo.Domain.Entities.Demo;

public class Log
{
    public int Id { get; set; }
    public string InsertedBy { get; set; }
    public DateTimeOffset InsertedOn { get; set; }

    public LogType LogType { set; get; }
    public ActionType ActionType { set; get; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string MethodName { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string UniqueObjectId { get; set; } = string.Empty;
    public string TraceId { get; set; }
}