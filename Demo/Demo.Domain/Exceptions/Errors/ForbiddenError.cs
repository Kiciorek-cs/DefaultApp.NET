using System;
using System.Net;

namespace Demo.Domain.Exceptions.Errors;

public class ForbiddenError : Exception
{
    public ForbiddenError(string message) : base($"{message}")
    {
    }

    public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.Forbidden;
}