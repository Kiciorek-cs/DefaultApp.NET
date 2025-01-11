using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Domain.Exceptions.Errors;

public class UnauthorizedObjectResultError : ObjectResult
{
    public UnauthorizedObjectResultError()
        : base(new { Message = "Unauthorized", ErrorCode = "401" }) 
    {
        StatusCode = StatusCodes.Status401Unauthorized;
    }

    public UnauthorizedObjectResultError(object value)
        : base(value)
    {
        StatusCode = StatusCodes.Status401Unauthorized;
    }
}