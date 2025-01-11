using Asp.Versioning;
using Demo.BLL.Features.Logs.Queries.GetByGuid;
using Demo.BLL.Features.Logs.Queries.GetList;
using Demo.Domain.Exceptions.Errors;
using Demo.Domain.Models.DTOModels.Log;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.API.Controllers.V2;

[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
//[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
public class LogsController : BaseController
{
    public LogsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    ///     Get all logs
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is a logs list</returns>
    /// <response code="200">Return data</response>
    /// <response code="500">Exception</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetListQuery(), cancellationToken));
    }

    /// <summary>
    ///     Get one log by guid
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is a logs</returns>
    /// <response code="200">Return data</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpGet("{guid}")]
    [ProducesResponseType(typeof(LogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByGuid(string guid,
        CancellationToken cancellationToken)
    {
        var model = new GetByGuidQuery { Guid = guid };

        return Ok(await _mediator.Send(model, cancellationToken));
    }
}