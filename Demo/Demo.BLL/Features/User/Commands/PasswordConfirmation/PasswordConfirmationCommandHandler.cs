using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Helpers.Permission.Password;
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

namespace Demo.BLL.Features.User.Commands.PasswordConfirmation;

public class PasswordConfirmationCommandHandler : IRequestHandler<PasswordConfirmationCommand, IResponse>
{
    private readonly ILogServices _logServices;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;
    private readonly IToken _token;

    public PasswordConfirmationCommandHandler(IPermissionUnitOfWork permissionUnitOfWork, IToken token,
        ILogServices logServices)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
        _token = token;
        _logServices = logServices;
    }

    public async Task<IResponse> Handle(PasswordConfirmationCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _permissionUnitOfWork.UserRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var (_, decodedInformation) = _token.GetTokenInformation(request.ResetToken, cancellationToken);

            var resetToken = await _permissionUnitOfWork.TokenRepository.GetAsync(
                x => x.Value == request.ResetToken && x.StatusType == TokenStatusType.Active &&
                     x.TokenType == TokenType.Reset,
                cancellationToken);

            var login = resetToken.Login;
            var user = resetToken.Login.User;

            if (user.Id != decodedInformation.UserId)
                throw new ValidationException("User data are not correct.", new List<CustomValidationFailure>
                {
                    new(null, user.Id, "User data are not correct.", ValidationKeys.WrongLogin)
                });

            var sHashedPassword = PasswordHashing.HashUsingPbkdf2(request.Password, login.PasswordSalt);
            login.PasswordHash = sHashedPassword;

            await _permissionUnitOfWork.CommitAsync(cancellationToken);

            var allTokens = await _permissionUnitOfWork.TokenRepository.GetMultipleAsync(
                x => x.LoginId == login.Id && x.StatusType == TokenStatusType.Active,
                cancellationToken);

            foreach (var token in allTokens) token.StatusType = TokenStatusType.Inactive;
            
            await _permissionUnitOfWork.CommitAsync(cancellationToken);

            await _logServices.AddLogToDatabase(ActionType.PUT, LogType.Update, "PasswordConfirmation", user, "Handle",
                cancellationToken, user.Login.EmailAddress);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = user.Id,
                Message = "Password has been changed.",
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