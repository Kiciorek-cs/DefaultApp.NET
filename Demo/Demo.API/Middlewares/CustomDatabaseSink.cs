using System.Linq;
using Demo.BLL.Helpers.Converter;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Entities.Demo;
using Demo.Domain.Enums;
using Demo.Infrastructure.Persistence.DatabaseContext.DapperContext;
using Demo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace Demo.API.Middlewares;

public class CustomDatabaseSink : ILogEventSink
{
    private readonly string _connectionString;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomDatabaseSink(string connectionString, IHttpContextAccessor httpContextAccessor)
    {
        _connectionString = connectionString;
        _httpContextAccessor = httpContextAccessor;
    }

    public void Emit(LogEvent logEvent)
    {
        var logMessage = logEvent.RenderMessage();
        var logLevel = logEvent.Level.ToString();
        var logTimestamp = logEvent.Timestamp;
        var logTraceId = logEvent.TraceId;

        var properties = logEvent.Properties;

        var requestType = string.Empty;
        var machineName = string.Empty;

        if (properties.TryGetValue("RequestMethod", out var userIdValue))
        {
            requestType = userIdValue.ToString().Trim('"');
        }
        else
        {
            var request = logMessage;
            var parts = request.Split(' ');
            requestType = parts.ElementAtOrDefault(1);
        }

        if (properties.TryGetValue("MachineName", out var machineNameValue))
            machineName = machineNameValue.ToString().Trim('"');

        var demoUnitOfWork = CreateCustomDemoUnitOfWork();

        var traceId = _httpContextAccessor.HttpContext?.TraceIdentifier;

        var newLog = new Log
        {
            InsertedBy = "Serilog",
            InsertedOn = logTimestamp,
            LogType = logLevel.StringToEnum<LogType>(),
            ActionType = requestType.StringToEnum<ActionType>(),
            Description = $"machineName: {machineName} - {logTraceId} - {logMessage}",
            Name = "Serilog data",
            MethodName = $"{logMessage}",
            TraceId = traceId ?? string.Empty
        };

        demoUnitOfWork.LogDapperRepository.InsertLog(newLog).GetAwaiter().GetResult();
    }

    private IDemoUnitOfWork CreateCustomDemoUnitOfWork()
    {
        var demoDapperContext = new DemoDapperContext(_connectionString);

        return new DemoUnitOfWork(null, demoDapperContext, null);
    }
}