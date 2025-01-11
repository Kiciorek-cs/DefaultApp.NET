using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Integrations.Email.Notifications;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.BLL.Interfaces.Services.PermissionServices;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.Domain.Enums;
using Demo.Domain.Enums.Permission;
using MediatR;

namespace Demo.BLL.Features.User.Commands.PasswordReset;

public class PasswordResetCommandHandler : IRequestHandler<PasswordResetCommand, IResponse>
{
    private readonly IEmailMessageService _emailMessageService;
    private readonly ILogServices _logServices;
    private readonly IPermission _permission;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;
    private readonly IToken _token;

    public PasswordResetCommandHandler(IPermissionUnitOfWork permissionUnitOfWork, IPermission permission,
        ILogServices logServices, IEmailMessageService emailMessageService, IToken token)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
        _permission = permission;
        _logServices = logServices;
        _emailMessageService = emailMessageService;
        _token = token;
    }

    public async Task<IResponse> Handle(PasswordResetCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _permissionUnitOfWork.UserRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = await _permissionUnitOfWork.UserRepository.GetAsync(
                x => x.Login.EmailAddress == request.EmailAddress,
                cancellationToken);

            var previousResetToken = user.Login.Tokens.FirstOrDefault(x =>
                x.StatusType == TokenStatusType.Active && x.TokenType == TokenType.Reset);

            if (previousResetToken != null)
                previousResetToken.StatusType = TokenStatusType.Inactive;

            var addedToken =
                await _permission.AddConfirmationToken(user, user.Login, TokenType.Reset, cancellationToken);

            await _logServices.AddLogToDatabase(ActionType.POST, LogType.Create, "PasswordReset", user, "Handle",
                cancellationToken, user.Login.EmailAddress);

            await _emailMessageService.SendPasswordResetConfirmationEmail(
                _token.GeneratePasswordConfirmationLink(addedToken.Value),
                user.Login.EmailAddress, cancellationToken);

            await _logServices.AddLogToDatabase(ActionType.POST, LogType.Email, "EmailWasSendUser",
                $"Password change confirmation email has been sent to {user.Login.EmailAddress}.", "Handle",
                cancellationToken, user.Login.EmailAddress);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = user.Id,
                Message = "Reset email has been send.",
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