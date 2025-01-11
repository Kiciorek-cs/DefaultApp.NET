using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.Integrations.Http.Client;
using Demo.BLL.Interfaces.Integrations.Http.DemoService;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.Domain.Exceptions.HttpIntegrationExceptions.Demo;
using Demo.Integration.Http.Client;

namespace Demo.Integration.Http.Services.DemoService;

public class DemoService : Service, IDemoService
{
    public DemoService(IHttpClient client, IClock clock) :
        base(client, clock) 
    {
    }

    public async Task<string> GetData(string ipAddress, string path)
    {
            var parameters = new Dictionary<string, string>() { };
            var header = new Dictionary<string, string>() { };

            var uri = new RequestUri(host: "https://test.com", parameters: parameters);

            var projects = await Request(() => _httpClient.Get<string>(uri, header),
                (message) => new DemoExceptions(message));

            return projects;
    }
}