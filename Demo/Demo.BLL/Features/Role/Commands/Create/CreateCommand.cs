using System.Collections.Generic;
using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.Role.Commands.Create;

public class CreateCommand : IRequest<IResponse>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsAccessWithoutObjectId { get; set; }
    public bool IsDefaultRole { get; set; }
    public List<int> Resources { get; set; }
}