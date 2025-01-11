using System.Collections.Generic;
using Demo.Domain.Models.DTOModels.Log;
using MediatR;

namespace Demo.BLL.Features.Logs.Queries.GetList;

public class GetListQuery : IRequest<IEnumerable<LogDto>>
{
}