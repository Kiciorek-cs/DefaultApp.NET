using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Demo.API.Middlewares;

public class TraceIdMiddleware
{
    private readonly RequestDelegate _next;

    public TraceIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.TraceIdentifier = Guid.NewGuid().ToString();
        var id = context.TraceIdentifier;
        context.Response.Headers["X-Trace-Id"] = id;
        await _next(context);
    }
}