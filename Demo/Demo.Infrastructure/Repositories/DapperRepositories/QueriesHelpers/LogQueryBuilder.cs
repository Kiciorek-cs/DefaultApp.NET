using Dapper;
using Demo.Domain.Entities.Demo;
using System.Data;
using System.Globalization;

namespace Demo.Infrastructure.Repositories.DapperRepositories.QueriesHelpers;

public static class LogQueryBuilder
{
    /// <summary>
    ///     Insert log
    /// </summary>
    /// <param name="log"></param>
    /// <returns></returns>
    public static CommandDefinition InsertLog(Log log, IDbTransaction contextTransaction = null)
    {
        const string dtFormat = "yyyy-MM-dd HH:mm:ss.fffffff zzz";

        var convertedTime = log.InsertedOn.ToString(dtFormat, CultureInfo.InvariantCulture);

        var commandText =
            @"INSERT INTO [dbo].[Logs]
                       (
                        [InsertedBy]
                       ,[InsertedOn]
                       ,[LogType]
                       ,[ActionType]
                       ,[Name]
                       ,[Description]
                       ,[MethodName]
                       ,[TraceId]
                       ,[TableName]
                       ,[UniqueObjectId]
                       )
                  OUTPUT INSERTED.Id
                  VALUES
                       (
                        @InsertedBy,
                        @InsertedOn,
                        @LogType, 
                        @ActionType, 
                        @Name,
                        @Description,
                        @MethodName,
                        @TraceId,
                        @TableName,
                        @UniqueObjectId
                       )";

        object parameters = new
        {
            log.InsertedBy,
            InsertedOn = convertedTime,
            LogType = log.LogType.ToString(),
            ActionType = log.ActionType.ToString(),
            log.Name,
            log.Description,
            log.MethodName,
            log.TraceId,
            log.TableName,
            log.UniqueObjectId
        };

        return new CommandDefinition(commandText, parameters, contextTransaction);
    }
}