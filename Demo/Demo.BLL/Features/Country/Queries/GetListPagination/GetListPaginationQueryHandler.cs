using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Models.DTOModels.Country;
using Demo.Domain.Models.HelperModels.Pagination;
using MediatR;

namespace Demo.BLL.Features.Country.Queries.GetListPagination;

public class
    GetListPaginationQueryHandler : IRequestHandler<GetListPaginationQuery, PaginationModelDto<CountryPaginationDto>>
{
    private readonly IDemoUnitOfWork _demoUnitOfWork;

    public GetListPaginationQueryHandler(IDemoUnitOfWork demoUnitOfWork)
    {
        _demoUnitOfWork = demoUnitOfWork;
    }

    public async Task<PaginationModelDto<CountryPaginationDto>> Handle(GetListPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var countries =
            await _demoUnitOfWork.CountryRepository.GetWithPagination(cancellationToken);

        return countries;
    }
}