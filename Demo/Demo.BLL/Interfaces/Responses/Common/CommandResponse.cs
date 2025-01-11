using Demo.BLL.Interfaces.CQRS;

namespace Demo.BLL.Interfaces.Responses.Common;

public class CommandResponse : BaseResponse, IResponse
{
    public int Id { get; set; }
}