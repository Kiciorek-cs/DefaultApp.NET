using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Domain.Exceptions.Errors;

public class ForbiddenObjectResultError : ObjectResult
{
    public ForbiddenObjectResultError()
        : base(new { Message = "Forbidden", ErrorCode = "403" }) 
    {
        StatusCode = StatusCodes.Status403Forbidden;
    }

    public ForbiddenObjectResultError(object value)
        : base(value)
    {
        StatusCode = StatusCodes.Status403Forbidden;
    }
}