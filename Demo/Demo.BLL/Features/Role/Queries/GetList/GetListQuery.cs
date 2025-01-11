using System.Collections.Generic;
using Demo.Domain.Models.DTOModels.Role;
using MediatR;

namespace Demo.BLL.Features.Role.Queries.GetList;

public class GetListQuery : IRequest<IEnumerable<RoleDto>>
{
}