using System.Collections.Generic;
using Demo.Domain.Models.DTOModels.Resource;
using MediatR;

namespace Demo.BLL.Features.Resource.Queries.GetList;

public class GetListQuery : IRequest<IEnumerable<ResourceDto>>
{
}