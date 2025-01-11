using Demo.BLL.Interfaces.Integrations.Email.Client;
using Demo.BLL.Interfaces.Integrations.Email.Notifications;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Integration.Email.Services.Notifications;

public class EmailMessageService : IEmailMessageService
{
    private const string Mainfolder = "\\Demo\\";
    private const string MainProjects = "Demo.API\\";

    private const string AppDockerFolder = "/app";

    private const string AccountConfirmation = "HTMLTemplates/AccountConfirmation/AccountConfirmationEmail.html";
    private const string PasswordResetConfirmation = "HTMLTemplates/PasswordResetConfirmation/PasswordResetConfirmationEmail.html";

    private readonly IEmailService _emailService;

    public EmailMessageService(IEmailService emailService)
    {
        _emailService = emailService;
        
    }

    public async Task<bool> SendEmailWithStringMessage(string message, string error, IEnumerable<string> emails,
        CancellationToken cancellationToken = default)
    {
        var body = $"<h3>{message}</h3><p>{error}</p>";

        return await _emailService.SendEmailAsync(emails, "Demo", "Błąd pobierania danych.", body,
            cancellationToken: cancellationToken);
    }

    public async Task<bool> SendAccountConfirmationEmail(string link, string email,
        CancellationToken cancellationToken = default)
    {
        var body = StreamReaderHtml(AccountConfirmation);
        if (body == null) return false;

        body = body.Replace("{LINK}", link);

        return await _emailService.SendEmailAsync(new List<string> { email }, "Kompostownia", "Potwierdzenie konta.",
            body, cancellationToken: cancellationToken);
    }

    public async Task<bool> SendPasswordResetConfirmationEmail(string link, string email,
        CancellationToken cancellationToken = default)
    {
        var body = StreamReaderHtml(PasswordResetConfirmation);
        if (body == null) return false;

        body = body.Replace("{LINK}", link);

        return await _emailService.SendEmailAsync(new List<string> { email }, "Kompostownia", "Resetowanie hasła.",
            body, cancellationToken: cancellationToken);
    }

    private static string StreamReaderHtml(string path)
    {
        try
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var pathToEmailTemaplate = string.Empty;

            Log.Warning(currentDirectory);

            if (currentDirectory.Contains(AppDockerFolder))
                pathToEmailTemaplate = Path.Combine(currentDirectory, path);
            else
                pathToEmailTemaplate =
                    Path.Combine(
                        currentDirectory.Substring(0, currentDirectory.LastIndexOf(Mainfolder) + Mainfolder.Length - 1),
                        MainProjects + path);

            using var reader = new StreamReader(pathToEmailTemaplate, Encoding.UTF8);

            return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception during reading email template.");
            return null;
        }
    }
}