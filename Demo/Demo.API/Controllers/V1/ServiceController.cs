using Asp.Versioning;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.Domain.ConfigurationModels.StaticModels;
using Demo.Domain.Exceptions.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.API.Controllers.V1;

[ApiVersion("1.0")]
//[Authorization(resourcesNames: "ADMIN")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ServiceController : BaseController
{
    public ServiceController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    ///     Set redirection property
    /// </summary>
    /// <param name="userEmails"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value redirection property</returns>
    /// <response code="200">Return data</response>
    /// <response code="500">Exception</response>
    [HttpPut("redirection")]
    //[Authorization(resourcesNames: "Email_Redirection")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Restart(List<string> userEmails, CancellationToken cancellationToken)
    {
        GlobalStaticVariable.EmailRedirection = string.Join(";", userEmails);

        return Ok(new CommandResponse
        {
            Message = GlobalStaticVariable.EmailRedirection
        });
    }
}