using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.User.Commands.ConfirmAccountByUserId;

public class ConfirmAccountByUserIdCommand : IRequest<IResponse>
{
    public int UserId { get; set; }
}