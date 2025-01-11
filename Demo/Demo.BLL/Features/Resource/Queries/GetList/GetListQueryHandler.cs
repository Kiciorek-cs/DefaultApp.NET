using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Models.DTOModels.Resource;
using MediatR;

namespace Demo.BLL.Features.Resource.Queries.GetList;

public class
    GetListQueryHandler : IRequestHandler<GetListQuery, IEnumerable<ResourceDto>>
{
    private readonly IMapper _mapper;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public GetListQueryHandler(
        IMapper mapper, IPermissionUnitOfWork permissionUnitOfWork)
    {
        _mapper = mapper;
        _permissionUnitOfWork = permissionUnitOfWork;
    }

    public async Task<IEnumerable<ResourceDto>> Handle(GetListQuery request,
        CancellationToken cancellationToken)
    {
        var resources =
            await _permissionUnitOfWork.ResourceRepository.GetMultipleNoTrackingAsync(null, cancellationToken);

        return _mapper.Map<IEnumerable<ResourceDto>>(resources);
    }
}