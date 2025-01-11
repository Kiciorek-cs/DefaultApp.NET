using System;
using System.Net.Http;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.Integrations.Http.Client;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Services.ErrorLogger;

namespace Demo.Integration.Http.Services;

public abstract class Service
{
    protected IClock _clock;
    protected IHttpClient _httpClient;

    protected Service(IHttpClient client, IClock clock)
    {
        _httpClient = client;
        _clock = clock;
    }

    public virtual async Task<TReturned> Request<TReturned>(Func<Task<TReturned>> requestMethod,
        Func<string, Exception> exception)
    {
        try
        {
            return await requestMethod();
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(
                _clock.Current(),
                $"Error while communicating with plc api: {ex.Message}");

            throw exception(ex.Message);
        }
    }
}