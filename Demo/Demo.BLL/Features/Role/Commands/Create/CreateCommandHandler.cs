using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Helpers.String;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.Domain.Enums;
using MediatR;

namespace Demo.BLL.Features.Role.Commands.Create;

public class CreateCommandHandler : IRequestHandler<CreateCommand, IResponse>
{
    private readonly ILogServices _logServices;
    private readonly IMapper _mapper;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public CreateCommandHandler(
        IMapper mapper, ILogServices logServices, IPermissionUnitOfWork permissionUnitOfWork)
    {
        _mapper = mapper;
        _logServices = logServices;
        _permissionUnitOfWork = permissionUnitOfWork;
    }

    public async Task<IResponse> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _permissionUnitOfWork.RoleRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            request.TrimStrings();

            var role = _mapper.Map<Domain.Entities.Permission.Role>(request);

            var resources =
                await _permissionUnitOfWork.ResourceRepository.GetMultipleAsync(x => request.Resources.Contains(x.Id),
                    cancellationToken);

            role.Resources = resources.ToList();

            var added =
                await _permissionUnitOfWork.RoleRepository.AddAsync(role, cancellationToken);

            await _permissionUnitOfWork.CommitAsync(cancellationToken);

            await _logServices.AddLogToDatabase(ActionType.POST, LogType.Create, "CreatePermissionRole", added,
                "Handle", cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = added.Id,
                Message = "The role has been added.",
                Successful = true
            };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}