using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.User.Commands.Delete;

public class DeleteCommand : IRequest<IResponse>
{
    public int Id { get; set; }
}