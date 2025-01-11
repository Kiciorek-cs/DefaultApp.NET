using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.User.Commands.ConfirmAccount;

public class ConfirmAccountCommand : IRequest<IResponse>
{
    public string ConfirmationToken { get; set; }
    public int UserId { get; set; }
}