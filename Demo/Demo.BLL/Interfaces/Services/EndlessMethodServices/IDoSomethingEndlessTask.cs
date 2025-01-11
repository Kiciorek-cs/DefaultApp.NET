using System.Threading;
using System.Threading.Tasks;

namespace Demo.BLL.Interfaces.Services.EndlessMethodServices;

public interface IDoSomethingEndlessTask
{
    Task ClearAndRunBackgroundJob(string text, int daley, CancellationToken cancellationToken);
    Task ClearAndRunBackgroundJobs(CancellationToken cancellationToken);
}