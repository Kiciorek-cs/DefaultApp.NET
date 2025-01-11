using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.BLL.Validators;
using Demo.Domain.Enums;
using Demo.Domain.Enums.Permission;
using FluentValidation;
using MediatR;

namespace Demo.BLL.Features.User.Commands.ConfirmAccount;

public class ConfirmAccountCommandHandler : IRequestHandler<ConfirmAccountCommand, IResponse>
{
    private readonly ILogServices _logServices;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;
    private readonly IToken _token;

    public ConfirmAccountCommandHandler(IPermissionUnitOfWork permissionUnitOfWork, IToken token,
        ILogServices logServices)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
        _token = token;
        _logServices = logServices;
    }

    public async Task<IResponse> Handle(ConfirmAccountCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _permissionUnitOfWork.UserRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var (_, decodedInformation) = _token.GetTokenInformation(request.ConfirmationToken, cancellationToken);

            var user = await _permissionUnitOfWork.UserRepository.GetAsync(x => x.Id == request.UserId,
                cancellationToken);

            if (user.Id != decodedInformation.UserId) //todo can move this to validation
                throw new ValidationException("User data are not correct.", new List<CustomValidationFailure>
                {
                    new(null, user.Id, "User data are not correct.", ValidationKeys.WrongLogin)
                });

            var confirmationToken = user.Login.Tokens.FirstOrDefault(x =>
                x.StatusType == TokenStatusType.Active && x.TokenType == TokenType.Confirmation);
            if (confirmationToken is null)
                throw new ValidationException("Configuration token doesn't exist.", new List<CustomValidationFailure>
                {
                    new(null, null, "Configuration token doesn't exist.", ValidationKeys.WrongLogin)
                });

            if (confirmationToken.Value != request.ConfirmationToken)
                throw new ValidationException("Token isn't valid.", new List<CustomValidationFailure>
                {
                    new(null, null, "Token isn't valid.", ValidationKeys.WrongLogin)
                });

            confirmationToken.StatusType = TokenStatusType.Inactive;
            user.Login.Status = AccountStatusType.Active;
            user.Login.EmailValidationStatus = EmailValidationStatusType.Confirmed;

            await _permissionUnitOfWork.CommitAsync(cancellationToken);

            await _logServices.AddLogToDatabase(ActionType.PUT, LogType.Update, "ConfirmAccount", user, "Handle",
                cancellationToken, user.Login.EmailAddress);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = user.Id,
                Message = "Account has been activated.",
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