using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.Domain.Enums;
using MediatR;

namespace Demo.BLL.Features.User.Commands.Update;

public class UpdateCommandHandler : IRequestHandler<UpdateCommand, IResponse>
{
    private readonly ILogServices _logServices;
    private readonly IMapper _mapper;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;

    public UpdateCommandHandler(IPermissionUnitOfWork permissionUnitOfWork, IMapper mapper, ILogServices logServices)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
        _mapper = mapper;
        _logServices = logServices;
    }

    public async Task<IResponse> Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _permissionUnitOfWork.UserRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = await _permissionUnitOfWork.UserRepository.GetAsync(x => x.Id == request.Id,
                cancellationToken);

            _mapper.Map(request, user);
            user.Login.LoginName = request.LoginName;

            await _permissionUnitOfWork.CommitAsync(cancellationToken);

            await _logServices.AddLogToDatabase(ActionType.PUT, LogType.Update, "UpdateUser", user, "Handle",
                cancellationToken, user.Login.EmailAddress);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = user.Id,
                Message = "User has been updated.",
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