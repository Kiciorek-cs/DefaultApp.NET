using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Models.DTOModels.Resource;
using MediatR;

namespace Demo.BLL.Features.Resource.Queries.GetById;

public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, ResourceDto>
{
    private readonly IMapper _mapper;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public GetByIdQueryHandler(
        IMapper mapper, IPermissionUnitOfWork permissionUnitOfWork)
    {
        _mapper = mapper;
        _permissionUnitOfWork = permissionUnitOfWork;
    }

    public async Task<ResourceDto> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var resource =
            await _permissionUnitOfWork.ResourceRepository.GetAsync(x => x.Id == request.Id, cancellationToken);

        return _mapper.Map<ResourceDto>(resource);
    }
}