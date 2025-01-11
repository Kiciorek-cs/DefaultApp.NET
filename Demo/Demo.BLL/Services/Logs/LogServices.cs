using System;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.Domain.Entities.Demo;
using Demo.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Demo.BLL.Services.Logs;

public class LogServices : ILogServices
{
    private readonly IClock _clock;
    private readonly IDemoUnitOfWork _demoUnitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IToken _token;

    public LogServices(IDemoUnitOfWork demoUnitOfWork, IClock clock, IToken token,
        IHttpContextAccessor httpContextAccessor)
    {
        _demoUnitOfWork = demoUnitOfWork;
        _clock = clock;
        _token = token;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task AddLogToDatabase<T>(ActionType actionType, LogType logType, string name, T description,
        string method, CancellationToken cancellationToken, string insertedBy = null)
    {
        string insertedByValue;

        if (string.IsNullOrEmpty(insertedBy))
        {
            var (_, claim) = _token.GetTokenInformationFromHeader(cancellationToken);
            insertedByValue = claim.EmailAddress;
        }
        else
        {
            insertedByValue = insertedBy;
        }

        var id = 0;
        var propertyInfo =
            typeof(T).GetProperty("id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo is not null && description is not null)
        {
            var value = propertyInfo.GetValue(description, null);
            if (value != null)
                id = Convert.ToInt32(value);
        }

        var traceId = _httpContextAccessor.HttpContext?.TraceIdentifier;

        await _demoUnitOfWork.LogRepository.AddAsync(new Log
        {
            ActionType = actionType,
            Description = typeof(T) == typeof(string) ? description.ToString() : JsonSerializer.Serialize(description),
            InsertedBy = insertedByValue,
            InsertedOn = _clock.Current(),
            LogType = logType,
            MethodName = method,
            Name = name,
            TableName = description is not null ? description.GetType().Name : "Description is null",
            UniqueObjectId = propertyInfo == null ? string.Empty : id.ToString(),
            TraceId = traceId ?? string.Empty
        }, cancellationToken);

        await _demoUnitOfWork.CommitAsync(cancellationToken);
    }
}