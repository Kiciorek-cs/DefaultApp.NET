using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.BLL.Interfaces.Integrations.Http.Client;

public interface IHttpClient
{
    Task<TResponse> Get<TResponse>(IRequestUri uri, Dictionary<string, string> header);
    Task<string> GetCsvFileAsString(IRequestUri uri, Dictionary<string, string> header);
    Task<TResponse> Post<TResponse, TBody>(IRequestUri uri, Dictionary<string, string> header, TBody body);
    Task<TResponse> Put<TResponse, TBody>(IRequestUri uri, Dictionary<string, string> header, TBody body);
    Task<TResponse> Patch<TResponse, TBody>(IRequestUri uri, Dictionary<string, string> header, TBody body);
    Task<TResponse> Delete<TResponse>(IRequestUri uri, Dictionary<string, string> header);
}