using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.User.Commands.UnblockAccount;

public class UnblockAccountCommand : IRequest<IResponse>
{
    public int Id { get; set; }
}