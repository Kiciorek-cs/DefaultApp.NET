using Demo.BLL.Interfaces.CQRS;
using Demo.Domain.Enums;
using MediatR;

namespace Demo.BLL.Features.Resource.Commands.Create;

public class CreateCommand : IRequest<IResponse>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public HttpMethodType HttpMethod { get; set; }
}