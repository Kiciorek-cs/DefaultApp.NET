using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.BLL.Validators;
using Demo.Domain.Enums;
using FluentValidation;
using MediatR;

namespace Demo.BLL.Features.User.Commands.UpdateRole;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IResponse>
{
    private readonly ILogServices _logServices;
    private readonly IMapper _mapper;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;
    private readonly IToken _token;

    public UpdateRoleCommandHandler(IPermissionUnitOfWork permissionUnitOfWork, IMapper mapper,
        ILogServices logServices, IToken token)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
        _mapper = mapper;
        _logServices = logServices;
        _token = token;
    }

    public async Task<IResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _permissionUnitOfWork.UserRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = await _permissionUnitOfWork.UserRepository.GetAsync(x => x.Id == request.Id, cancellationToken);

            var (_, userFromToken) = _token.GetTokenInformationFromHeader(cancellationToken);

            if (userFromToken is null)
                throw new ValidationException("Token doesn't exist", new List<CustomValidationFailure>
                {
                    new(null, null, "Token doesn't exist", ValidationKeys.UserNotExist)
                });

            user.RoleId = request.RoleId;
            var updatedUser = _permissionUnitOfWork.UserRepository.Update(user);

            await _permissionUnitOfWork.CommitAsync(cancellationToken);

            await _logServices.AddLogToDatabase(ActionType.PUT, LogType.Update, "UpdateUser", user, "Handle",
                cancellationToken, userFromToken.EmailAddress);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = updatedUser.Id,
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