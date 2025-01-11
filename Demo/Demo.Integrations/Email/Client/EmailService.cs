using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Demo.BLL.Interfaces.Integrations.Email.Client;
using Demo.BLL.Interfaces.Services.Logs;
using Demo.Domain.ConfigurationModels.DynamicModel;
using Demo.Domain.ConfigurationModels.StaticModels;
using Demo.Domain.Enums;
using Demo.Domain.Models.DTOModels.File;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Serilog;
using Directory = System.IO.Directory;

namespace Demo.Integration.Email.Client;

public class EmailService : IEmailService
{
    private const string _mainFolder = "\\Demo\\";
    private const string _mainProjects = "Demo.API\\";
    private const string _appDockerFolder = "/app";

    private readonly IConfiguration _configuration;

    private readonly string _env;
    private readonly MicrosoftGraphMailSettings _graphMailSettings;
    private readonly ILogServices _logServices;
    private const int _maxChunkSize = 1024 * 320;


    public EmailService(IConfiguration configuration, ILogServices logServices)
    {
        _configuration = configuration;
        _env = _configuration["Environment"];
        _graphMailSettings = configuration.GetSection("MicrosoftGraphMailSettings").Get<MicrosoftGraphMailSettings>();
        _logServices = logServices;
    }

    /// <summary>
    ///     Old method for send email using SMTP
    /// </summary>
    /// <param name="sendTo"></param>
    /// <param name="displayName"></param>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    /// <param name="attachments"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns
    public async Task<bool> SendEmailAsync(IEnumerable<string> sendTo, string displayName, string subject, string body,
        List<FileDto> attachments = null, CancellationToken cancellationToken = default)
    {
        if (!_env.Equals("Production", StringComparison.OrdinalIgnoreCase))
        {
            await _logServices.AddLogToDatabase(ActionType.UNDEFINED, LogType.Email, "SendEmailAsync",
                "Emails are redirected.", "Handle", cancellationToken, string.Join(";", sendTo));

            if (string.IsNullOrEmpty(GlobalStaticVariable.EmailRedirection)) return true;
            var listOfEmails = GlobalStaticVariable.EmailRedirection.Split(";");
            sendTo = listOfEmails;
        }

        var toRecipients = new List<Recipient>();
        foreach (var email in sendTo)
        {
            var toRecipient = new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Address = email
                }
            };

            toRecipients.Add(toRecipient);
        }

        var message = new Message
        {
            Subject = subject,
            Body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = body
            },
            ToRecipients = toRecipients
        };

        try
        {
            await SendEmailSmtpClient(message, attachments, cancellationToken);

            Log.ForContext("subject", subject).Information("Successfully sent email.");

            return true;
        }
        catch (Exception ex)
        {
            Log.Error("Exception occurred during sending email.");
            throw ex;
        }
    }

    public string StreamReaderHtml(string path)
    {
        try
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            string pathToEmailTemplate;

            if (currentDirectory.Contains(_appDockerFolder))
                pathToEmailTemplate = Path.Combine(currentDirectory, path);
            else
                pathToEmailTemplate =
                    Path.Combine(
                        currentDirectory.Substring(0,
                            currentDirectory.LastIndexOf(_mainFolder) + _mainFolder.Length - 1), _mainProjects + path);

            using StreamReader reader = new(pathToEmailTemplate, Encoding.UTF8);

            return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception during reading email template.");
            return null;
        }
    }

    private async Task SendEmailSmtpClient(Message message, List<FileDto> attachments = null,
        CancellationToken cancellationToken = default)
    {
        var credentials = new ClientSecretCredential(
            _graphMailSettings.TenantId,
            _graphMailSettings.ClientId,
            _graphMailSettings.ClientSecret,
            new TokenCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud });

        var graphServiceClient = new GraphServiceClient(credentials);

        //Send email with attachment< 3 mb
        if (attachments == null)
        {
            await graphServiceClient
                .Users[_graphMailSettings.UserObjectId]
                .SendMail(message, true)
                .Request()
                .WithMaxRetry(5)
                .PostAsync(cancellationToken);
        }
        //.Result;
        else
        {
            var msgResult = await graphServiceClient.Users[_graphMailSettings.UserObjectId].Messages.Request()
                .AddAsync(message, cancellationToken);
            foreach (var item in attachments)
            {
                var attachmentItem = new AttachmentItem
                {
                    AttachmentType = AttachmentType.File,
                    Name = item.FileName + "." + item.Extension,
                    Size = Convert.ToInt32(item.Size)
                };
                var uploadSession = await graphServiceClient.Users[_graphMailSettings.UserObjectId]
                    .Messages[msgResult.Id].Attachments
                    .CreateUploadSession(attachmentItem)
                    .Request()
                    .PostAsync(cancellationToken);

                using var stream = new MemoryStream(item.File);
                stream.Position = 0;

                var largeFileUploadTask = new LargeFileUploadTask<FileAttachment>(uploadSession, stream, _maxChunkSize);
                await largeFileUploadTask.UploadAsync();
            }

            await graphServiceClient.Users[_graphMailSettings.UserObjectId].Messages[msgResult.Id].Send().Request()
                .PostAsync(cancellationToken);
        }
    }
}