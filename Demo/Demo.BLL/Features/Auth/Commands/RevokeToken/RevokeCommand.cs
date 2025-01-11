using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.Auth.Commands.RevokeToken;

public class RevokeCommand : IRequest<IResponse>
{
    public string AccessToken { get; set; }
}