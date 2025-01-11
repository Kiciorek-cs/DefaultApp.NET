using System.Collections.Generic;
using System.Text.Json;

namespace Demo.Domain.Exceptions.Errors;

public class ValidationError
{
    public int StatusCode { get; set; }

    public bool Successfull { get; set; } = false;

    public IEnumerable<ValidationErrorDetails> Errors { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

public class ValidationErrorDetails
{
    public string Property { get; set; }
    public string AttemptedValue { get; set; }
    public string Message { get; set; }
    public string Error { get; set; }
    public string KeyValidator { get; set; }
}