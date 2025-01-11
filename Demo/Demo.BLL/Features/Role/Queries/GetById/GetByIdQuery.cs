using Demo.Domain.Models.DTOModels.Role;
using MediatR;

namespace Demo.BLL.Features.Role.Queries.GetById;

public class GetByIdQuery : IRequest<RoleDto>
{
    public int Id { get; set; }
}