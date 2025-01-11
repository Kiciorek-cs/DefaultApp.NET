namespace Demo.BLL.Interfaces.CQRS;

public class BaseResponse
{
    public BaseResponse()
    {
        Successful = true;
    }

    public BaseResponse(bool successful)
    {
        Successful = successful;
    }

    public BaseResponse(bool successful, string message)
    {
        Successful = successful;
        Message = message;
    }

    public bool Successful { get; set; }
    public string Message { get; set; }
}