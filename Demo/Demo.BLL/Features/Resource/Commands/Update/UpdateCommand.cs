using Demo.BLL.Interfaces.CQRS;
using Demo.Domain.Enums;
using MediatR;

namespace Demo.BLL.Features.Resource.Commands.Update;

public class UpdateCommand : IRequest<IResponse>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public HttpMethodType HttpMethod { get; set; }
}