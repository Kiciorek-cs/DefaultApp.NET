using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Models.DTOModels.Country;
using MediatR;

namespace Demo.BLL.Features.Country.Queries.GetById;

public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, CountryDto>
{
    private readonly IDemoUnitOfWork _demoUnitOfWork;
    private readonly IMapper _mapper;

    public GetByIdQueryHandler(
        IMapper mapper, IDemoUnitOfWork demoUnitOfWork)
    {
        _mapper = mapper;
        _demoUnitOfWork = demoUnitOfWork;
    }

    public async Task<CountryDto> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var countryDto =
            await _demoUnitOfWork.CountryRepository.GetAsync(x => x.Id == request.Id, cancellationToken);

        return _mapper.Map<CountryDto>(countryDto);
    }
}