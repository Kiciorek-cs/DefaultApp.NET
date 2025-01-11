using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.User.Commands.PasswordReset;

public class PasswordResetCommand : IRequest<IResponse>
{
    public string EmailAddress { get; set; }
}