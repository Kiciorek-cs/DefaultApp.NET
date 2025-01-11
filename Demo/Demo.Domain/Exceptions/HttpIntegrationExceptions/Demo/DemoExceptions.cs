using System;

namespace Demo.Domain.Exceptions.HttpIntegrationExceptions.Demo;

public class DemoExceptions : Exception
{
    public DemoExceptions(string message)
        : base($"{message}")
    {
    }
}