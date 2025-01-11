using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.User.Commands.PasswordConfirmation;

public class PasswordConfirmationCommand : IRequest<IResponse>
{
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string ResetToken { get; set; }
}