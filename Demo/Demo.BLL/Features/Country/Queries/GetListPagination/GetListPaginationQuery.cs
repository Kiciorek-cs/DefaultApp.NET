using Demo.Domain.Models.DTOModels.Country;
using Demo.Domain.Models.HelperModels.Pagination;
using MediatR;

namespace Demo.BLL.Features.Country.Queries.GetListPagination;

public class GetListPaginationQuery : IRequest<PaginationModelDto<CountryPaginationDto>>
{
}