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

namespace Demo.BLL.Features.Resource.Commands.Create;

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
        var transaction = await _permissionUnitOfWork.ResourceRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            request.TrimStrings();

            var resource = _mapper.Map<Domain.Entities.Permission.Resource>(request);

            var added =
                await _permissionUnitOfWork.ResourceRepository.AddAsync(resource, cancellationToken);

            await _permissionUnitOfWork.CommitAsync(cancellationToken);

            await _logServices.AddLogToDatabase(ActionType.POST, LogType.Create, "CreatePermissionResource", added,
                "Handle", cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = added.Id,
                Message = "The resource has been added.",
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