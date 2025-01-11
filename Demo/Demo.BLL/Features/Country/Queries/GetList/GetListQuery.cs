using System.Collections.Generic;
using Demo.Domain.Models.DTOModels.Country;
using MediatR;

namespace Demo.BLL.Features.Country.Queries.GetList;

public class GetListQuery : IRequest<IEnumerable<CountryDto>>
{
}