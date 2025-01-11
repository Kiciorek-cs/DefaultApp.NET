using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Demo.Domain.Models.DTOModels.File;

namespace Demo.BLL.Interfaces.Integrations.Email.Client;

public interface IEmailService
{
    Task<bool> SendEmailAsync(IEnumerable<string> sendTo, string displayName, string subject, string body,
        List<FileDto> attachments = null, CancellationToken cancellationToken = default);

    string StreamReaderHtml(string path);
}