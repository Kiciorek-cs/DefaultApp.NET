using System.Text.Json;

namespace Demo.Domain.Exceptions.Errors;

public class ErrorDetails
{
    public int StatusCode { get; set; }

    public string Message { get; set; } =
        "An unhandled exception occurred. Please contact your administrator for further assistance.\"";

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}