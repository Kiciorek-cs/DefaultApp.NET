using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Enums.Permission;
using Demo.Domain.Models.DTOModels.User;
using MediatR;

namespace Demo.BLL.Features.User.Queries.GetList;

public class
    GetListQueryHandler : IRequestHandler<GetListQuery, IEnumerable<UserDto>>
{
    private readonly IMapper _mapper;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public GetListQueryHandler(
        IMapper mapper, IPermissionUnitOfWork permissionUnitOfWork)
    {
        _mapper = mapper;
        _permissionUnitOfWork = permissionUnitOfWork;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetListQuery request,
        CancellationToken cancellationToken)
    {
        var listOfUser =
            await _permissionUnitOfWork.UserRepository.GetMultipleNoTrackingAsync(
                x => x.Login.Status != AccountStatusType.Deleted, cancellationToken);

        return _mapper.Map<IEnumerable<UserDto>>(listOfUser);
    }
}