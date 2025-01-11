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

namespace Demo.BLL.Features.Resource.Commands.Update;

public class UpdateCommandHandler : IRequestHandler<UpdateCommand, IResponse>
{
    private readonly ILogServices _logServices;
    private readonly IMapper _mapper;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public UpdateCommandHandler(
        IMapper mapper, ILogServices logServices, IPermissionUnitOfWork permissionUnitOfWork)
    {
        _mapper = mapper;
        _logServices = logServices;
        _permissionUnitOfWork = permissionUnitOfWork;
    }

    public async Task<IResponse> Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _permissionUnitOfWork.ResourceRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            request.TrimStrings();

            var resource =
                await _permissionUnitOfWork.ResourceRepository.GetAsync(x => x.Id == request.Id, cancellationToken);

            var mapped = _mapper.Map(request, resource);

            var update = _permissionUnitOfWork.ResourceRepository.Update(mapped);

            await _permissionUnitOfWork.CommitAsync(cancellationToken);

            await _logServices.AddLogToDatabase(ActionType.PUT, LogType.Update, "UpdatePermissionResource", update,
                "Handle", cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = update.Id,
                Message = "The resource has been updated.",
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