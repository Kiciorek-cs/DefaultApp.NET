using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.Role.Commands.Delete;

public class DeleteCommand : IRequest<IResponse>
{
    public int Id { get; set; }
}