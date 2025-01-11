using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Demo.API.PipelinesBehaviours;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;
        _logger.LogInformation($"Handler with name {requestName} was called.");

        TResponse response;
        try
        {
            Log.Information("Request parameters: {@Request}", request);
        }
        catch (NotSupportedException)
        {
            _logger.LogWarning("[Serialization ERROR] Could not serialize the request.");
        }

        response = await next();
        return response;
    }
}