using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.Integrations.Http.Client;
using Demo.Domain.Exceptions.HttpIntegrationExceptions;
using Newtonsoft.Json;
using Serilog;

namespace Demo.Integration.Http.Client;

public class HttpClientAdapter : IHttpClient
{
    private readonly HttpClient _client = new();

    public HttpClientAdapter()
    {
        _client.DefaultRequestHeaders.Clear();
    }

    public async Task<TResponse> Get<TResponse>(IRequestUri uri, Dictionary<string, string> header)
    {
        var url = uri.GetUrl();
        Log.Information("Call external service {uri}, header: {header}.", url, header);

        return await GeneralRequest<TResponse>(header, () => _client.GetAsync(url));
    }

    public async Task<string> GetCsvFileAsString(IRequestUri uri, Dictionary<string, string> header)
    {
        var url = uri.GetUrl();
        Log.Information("Call external service {uri}, header: {header}.", url, header);

        return await GeneralRequestCsvFileAsString(header, () => _client.GetAsync(url));
    }

    public async Task<TResponse> Post<TResponse, TBody>(IRequestUri uri, Dictionary<string, string> header,
        TBody body)
    {
        var url = uri.GetUrl();
        var json = JsonConvert.SerializeObject(body);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        Log.Information("Call external service {uri}, header: {header}, body: {json}", url, header, json);

        return await GeneralRequest<TResponse>(header, () => _client.PostAsync(url, data));
    }

    public async Task<TResponse> Put<TResponse, TBody>(IRequestUri uri, Dictionary<string, string> header,
        TBody body)
    {
        var url = uri.GetUrl();
        var json = JsonConvert.SerializeObject(body);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        Log.Information("Call external service {uri}, header: {header}, body: {json}", url, header, json);

        return await GeneralRequest<TResponse>(header, () => _client.PutAsync(url, data));
    }

    public async Task<TResponse> Patch<TResponse, TBody>(IRequestUri uri, Dictionary<string, string> header,
        TBody body)
    {
        var url = uri.GetUrl();
        var json = JsonConvert.SerializeObject(body);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        Log.Information("Call external service {uri}, header: {header}, body: {json}", url, header, json);

        return await GeneralRequest<TResponse>(header, () => _client.PatchAsync(url, data));
    }

    public async Task<TResponse> Delete<TResponse>(IRequestUri uri, Dictionary<string, string> header)
    {
        var url = uri.GetUrl();
        Log.Information("Call external service {uri}, header: {header}.", url, header);

        return await GeneralRequest<TResponse>(header, () => _client.DeleteAsync(url));
    }

    private async Task<TResponse> GeneralRequest<TResponse>(Dictionary<string, string> header,
        Func<Task<HttpResponseMessage>> method)
    {
        CreateHeader(header);
        var response = await method();
        try
        {
            response.EnsureSuccessStatusCode();
            var contentStream = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(contentStream);
            using var jsonReader = new JsonTextReader(streamReader);

            var serializer = new JsonSerializer();
            try
            {
                return serializer.Deserialize<TResponse>(jsonReader);
            }
            catch (JsonReaderException)
            {
                var responseStreamText = MapResponseStreamToString(contentStream);
                throw new ParseToObjectError(responseStreamText);
            }
        }
        catch (HttpRequestException ex)
        {
            var message = MapResponseStreamToString(await response.Content.ReadAsStreamAsync());
            throw new HttpCommunicationError(response.StatusCode.ToString(), $"{message} - {ex.Message}");
        }
    }

    private async Task<string> GeneralRequestCsvFileAsString(Dictionary<string, string> header,
        Func<Task<HttpResponseMessage>> method)
    {
        CreateHeader(header);
        try
        {
            var response = await method();
            try
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
            catch (HttpRequestException ex)
            {
                var message = MapResponseStreamToString(await response.Content.ReadAsStreamAsync());
                throw new HttpCommunicationError(response.StatusCode.ToString(), $"{message} - {ex.Message}");
            }
        }
        catch (HttpRequestException ex)
        {
            //string message = MapResponseStreamToString(await response.Content.ReadAsStreamAsync());
            //throw new HttpCommunicationError(response.StatusCode.ToString(), $"{message} - {ex.Message}");
        }

        return null;
    }

    private string MapResponseStreamToString(Stream responseStream)
    {
        return new StreamReader(responseStream).ReadToEnd();
    }

    private void CreateHeader(Dictionary<string, string> header)
    {
        _client.DefaultRequestHeaders.Clear();
        foreach (var (key, value) in header) _client.DefaultRequestHeaders.Add(key, value);
    }
}