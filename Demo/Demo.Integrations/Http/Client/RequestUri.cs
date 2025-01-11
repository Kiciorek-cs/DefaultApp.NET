using System.Collections.Generic;
using System.Linq;
using System.Text;
using Demo.BLL.Interfaces.Integrations.Http.Client;

namespace Demo.Integration.Http.Client;

public class RequestUri : IRequestUri
{
    private readonly string _host;
    private readonly List<KeyValuePair<string, string>> _keyValueParameters;
    private readonly string _mappedParameters;
    private readonly Dictionary<string, string> _parameters;
    private readonly string _resourcePath;

    public RequestUri(string host, string resourcePath)
    {
        _host = host;
        _resourcePath = resourcePath;
    }

    public RequestUri(string host, string resourcePath = null, Dictionary<string, string> parameters = null)
    {
        _host = host;
        _resourcePath = resourcePath;
        _parameters = parameters;

        if (_parameters != null)
            _mappedParameters =
                CreateParams(parameters.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToList());
    }

    public RequestUri(string host, string resourcePath = null, List<KeyValuePair<string, string>> parameters = null)
    {
        _host = host;
        _resourcePath = resourcePath;
        _keyValueParameters = parameters;

        if (_keyValueParameters != null) _mappedParameters = CreateParams(parameters);
    }

    public string GetUrl()
    {
        var path = new StringBuilder($"{_host}");

        if (_resourcePath is not null)
            path.Append($"/{_resourcePath}");

        if (_mappedParameters != null)
            path.Append($"?{_mappedParameters}");

        return path.ToString();
    }

    private string CreateParams(List<KeyValuePair<string, string>> parameters)
    {
        var mappedParameters = new List<string>();

        foreach (var kvp in parameters) mappedParameters.Add($"{kvp.Key}={kvp.Value}");

        if (mappedParameters.Count > 0) return string.Join("&", mappedParameters);
        return null;
    }
}