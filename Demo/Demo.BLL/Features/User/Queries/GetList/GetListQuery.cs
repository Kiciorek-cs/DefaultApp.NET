using System.Collections.Generic;
using Demo.Domain.Models.DTOModels.User;
using MediatR;

namespace Demo.BLL.Features.User.Queries.GetList;

public class GetListQuery : IRequest<IEnumerable<UserDto>>
{
}