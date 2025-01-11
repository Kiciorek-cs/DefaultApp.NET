using System.Threading;
using System.Threading.Tasks;
using Demo.Domain.Enums;

namespace Demo.BLL.Interfaces.Services.Logs;

public interface ILogServices
{
    Task AddLogToDatabase<T>(ActionType actionType, LogType logType, string name, T description, string method,
        CancellationToken cancellationToken, string insertedBy = null);
}