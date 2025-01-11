using System;

namespace Demo.Domain.Exceptions.HttpIntegrationExceptions;

public class HttpCommunicationError : Exception
{
    public HttpCommunicationError(string errorCode, string errorMessage)
        : base($"Error code {errorCode}, Error message {errorMessage}")
    {
    }
}

public class ParseToObjectError : Exception
{
    public ParseToObjectError(string json)
        : base($"Json {json} cannot be parsed to object")
    {
    }
}