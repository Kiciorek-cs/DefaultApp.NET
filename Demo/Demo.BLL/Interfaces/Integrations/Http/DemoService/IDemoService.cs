using System.Threading.Tasks;

namespace Demo.BLL.Interfaces.Integrations.Http.DemoService;

public interface IDemoService
{
    Task<string> GetData(string ipAddress, string path);
}