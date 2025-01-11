using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.Auth.Commands.RefreshToken;

public class RefreshCommand : IRequest<IResponse>
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}