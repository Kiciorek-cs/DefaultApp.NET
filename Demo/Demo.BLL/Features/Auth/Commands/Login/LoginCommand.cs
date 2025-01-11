using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<IResponse>
{
    public string EmailAddress { get; set; }
    public string Password { get; set; }
}