using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.Domain.Models.DTOModels.User;
using MediatR;

namespace Demo.BLL.Features.User.Queries.GetOwnAccount;

public class GetOwnAccountQueryHandler : IRequestHandler<GetOwnAccountQuery, UserDto>
{
    private readonly IMapper _mapper;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;
    private readonly IToken _token;

    public GetOwnAccountQueryHandler(
        IMapper mapper, IPermissionUnitOfWork permissionUnitOfWork, IToken token)
    {
        _mapper = mapper;
        _permissionUnitOfWork = permissionUnitOfWork;
        _token = token;
    }

    public async Task<UserDto> Handle(GetOwnAccountQuery request, CancellationToken cancellationToken)
    {
        var (_, decodedInformation) = _token.GetTokenInformationFromHeader(cancellationToken);

        var user = await _permissionUnitOfWork.UserRepository.GetAsync(x => x.Id == decodedInformation.UserId,
            cancellationToken);

        return _mapper.Map<UserDto>(user);
    }
}