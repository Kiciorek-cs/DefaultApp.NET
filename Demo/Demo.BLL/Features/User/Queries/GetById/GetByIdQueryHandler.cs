using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Interfaces.Repositories;
using Demo.Domain.Models.DTOModels.User;
using MediatR;

namespace Demo.BLL.Features.User.Queries.GetById;

public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, UserDto>
{
    private readonly IMapper _mapper;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public GetByIdQueryHandler(
        IMapper mapper, IPermissionUnitOfWork permissionUnitOfWork)
    {
        _mapper = mapper;
        _permissionUnitOfWork = permissionUnitOfWork;
    }

    public async Task<UserDto> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var user =
            await _permissionUnitOfWork.UserRepository.GetAsync(x => x.Id == request.Id, cancellationToken);

        return _mapper.Map<UserDto>(user);
    }
}