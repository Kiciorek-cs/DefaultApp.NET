using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Models.DTOModels.Country;
using MediatR;

namespace Demo.BLL.Features.Country.Queries.GetList;

public class
    GetListQueryHandler : IRequestHandler<GetListQuery, IEnumerable<CountryDto>>
{
    private readonly IDemoUnitOfWork _demoUnitOfWork;
    private readonly IMapper _mapper;

    public GetListQueryHandler(
        IMapper mapper, IDemoUnitOfWork demoUnitOfWork)
    {
        _mapper = mapper;
        _demoUnitOfWork = demoUnitOfWork;
    }

    public async Task<IEnumerable<CountryDto>> Handle(GetListQuery request,
        CancellationToken cancellationToken)
    {
        var listOfCountries =
            await _demoUnitOfWork.CountryRepository.GetMultipleNoTrackingAsync(null, cancellationToken);

        return _mapper.Map<IEnumerable<CountryDto>>(listOfCountries);
    }
}