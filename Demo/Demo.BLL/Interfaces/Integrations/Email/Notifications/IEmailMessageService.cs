using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.BLL.Interfaces.Integrations.Email.Notifications;

public interface IEmailMessageService
{
    Task<bool> SendEmailWithStringMessage(string message, string error, IEnumerable<string> emails,
        CancellationToken cancellationToken = default);

    Task<bool> SendAccountConfirmationEmail(string link, string email,
        CancellationToken cancellationToken = default);

    Task<bool> SendPasswordResetConfirmationEmail(string link, string email,
        CancellationToken cancellationToken = default);
}