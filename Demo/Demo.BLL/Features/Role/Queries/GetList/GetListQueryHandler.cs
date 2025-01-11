using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Models.DTOModels.Role;
using MediatR;

namespace Demo.BLL.Features.Role.Queries.GetList;

public class
    GetListQueryHandler : IRequestHandler<GetListQuery, IEnumerable<RoleDto>>
{
    private readonly IMapper _mapper;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public GetListQueryHandler(
        IMapper mapper, IPermissionUnitOfWork permissionUnitOfWork)
    {
        _mapper = mapper;
        _permissionUnitOfWork = permissionUnitOfWork;
    }

    public async Task<IEnumerable<RoleDto>> Handle(GetListQuery request,
        CancellationToken cancellationToken)
    {
        var roles =
            await _permissionUnitOfWork.RoleRepository.GetMultipleNoTrackingAsync(null, cancellationToken);

        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }
}