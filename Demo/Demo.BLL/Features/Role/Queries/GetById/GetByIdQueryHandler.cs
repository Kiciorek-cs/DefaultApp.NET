using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Models.DTOModels.Role;
using MediatR;

namespace Demo.BLL.Features.Role.Queries.GetById;

public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, RoleDto>
{
    private readonly IMapper _mapper;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public GetByIdQueryHandler(
        IMapper mapper, IPermissionUnitOfWork permissionUnitOfWork)
    {
        _mapper = mapper;
        _permissionUnitOfWork = permissionUnitOfWork;
    }

    public async Task<RoleDto> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var resource =
            await _permissionUnitOfWork.RoleRepository.GetAsync(x => x.Id == request.Id, cancellationToken);

        return _mapper.Map<RoleDto>(resource);
    }
}