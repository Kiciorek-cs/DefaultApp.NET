using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.CQRS;
using Demo.BLL.Interfaces.Integrations.Email.Notifications;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.BLL.Interfaces.Services.PermissionServices;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.Domain.Entities.Permission;
using Demo.Domain.Enums;
using Demo.Domain.Enums.Permission;
using MediatR;

namespace Demo.BLL.Features.User.Commands.Create;

public class CreateCommandHandler : IRequestHandler<CreateCommand, IResponse>
{
    private readonly IEmailMessageService _emailMessageService;

    private readonly ILogServices _logServices;
    private readonly IPermission _permission;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;
    private readonly IToken _token;

    public CreateCommandHandler(IPermissionUnitOfWork permissionUnitOfWork, IPermission permission,
        ILogServices logServices, IEmailMessageService emailMessageService, IToken token)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
        _permission = permission;
        _logServices = logServices;
        _emailMessageService = emailMessageService;
        _token = token;
    }

    public async Task<IResponse> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _permissionUnitOfWork.UserRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var (addedUser, addedLogin, addedToken) = await CreateUser(request, cancellationToken);

            await SendConfirmationEmail(addedUser, addedLogin, addedToken, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new CommandResponse
            {
                Id = addedUser.Id,
                Message = "User has been added.",
                Successful = true
            };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<(Domain.Entities.Permission.User user, Login login, Token token)> CreateUser(
        CreateCommand request, CancellationToken cancellationToken)
    {
        var addedUser = await _permission.AddUser(request, cancellationToken);

        var addedLogin = await _permission.AddLogin(request, addedUser, cancellationToken);

        var addedToken =
            await _permission.AddConfirmationToken(addedUser, addedLogin, TokenType.Confirmation, cancellationToken);

        await _logServices.AddLogToDatabase(ActionType.POST, LogType.Create, "CreateUser", addedUser, "Handle",
            cancellationToken, addedLogin.EmailAddress);

        await _permissionUnitOfWork.CommitAsync(cancellationToken);

        return (addedUser, addedLogin, addedToken);
    }

    private async Task SendConfirmationEmail(Domain.Entities.Permission.User user, Login login, Token token,
        CancellationToken cancellationToken)
    {
        await _emailMessageService.SendAccountConfirmationEmail(
            _token.GenerateConfirmationLink(user.Id, token.Value),
            login.EmailAddress, cancellationToken);

        await _logServices.AddLogToDatabase(ActionType.POST, LogType.Email, "EmailWasSendUser",
            $"Confirmation email has been sent to {login.EmailAddress}.", "Handle", cancellationToken,
            login.EmailAddress);
    }
}