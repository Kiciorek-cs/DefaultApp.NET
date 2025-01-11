using Demo.Domain.Models.DTOModels.Log;
using MediatR;
using System.Collections.Generic;

namespace Demo.BLL.Features.Logs.Queries.GetByGuid;

public class GetByGuidQuery : IRequest<IEnumerable<LogDto>>
{
    public string Guid { get; set; }
}