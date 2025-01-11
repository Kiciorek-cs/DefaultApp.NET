using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.User.Commands.SuspendAccount;

public class SuspendAccountCommand : IRequest<IResponse>
{
    public int Id { get; set; }
}