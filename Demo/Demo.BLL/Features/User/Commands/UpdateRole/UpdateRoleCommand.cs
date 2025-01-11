using Demo.BLL.Interfaces.CQRS;
using MediatR;

namespace Demo.BLL.Features.User.Commands.UpdateRole;

public class UpdateRoleCommand : IRequest<IResponse>
{
    public int Id { get; set; }
    public int RoleId { get; set; }
}