using Asp.Versioning;
using Demo.API.Middlewares.Permission;
using Demo.BLL.Features.BackgroundTask.Commands.Create;
using Demo.BLL.Features.BackgroundTask.Commands.Delete;
using Demo.BLL.Features.BackgroundTask.Commands.DeleteAll;
using Demo.BLL.Features.BackgroundTask.Queries.GetList;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.Domain.Exceptions.Errors;
using Demo.Domain.Models.DALModels.BackgroundTask;
using Demo.Domain.Models.DTOModels.BackgroundTask;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.API.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
//[Authorization(resourcesNames: "ADMIN")]
//[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
public class BackgroundTaskController : BaseController
{
    public BackgroundTaskController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    ///     Get all background tasks
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is a background task list</returns>
    /// <response code="200">Return data</response>
    /// <response code="500">Exception</response>
    [HttpGet]
    [Authorization(resourcesNames: "Get_All_Background_Task")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<BackgroundTaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetListQuery(), cancellationToken));
    }

    /// <summary>
    ///     Insert a background task
    /// </summary>
    /// <param name="backgroundTaskDal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is the id from the database</returns>
    /// <response code="201">Object has been created</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpPost]
    [Authorization(resourcesNames: "Create_Background_Task")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(BackgroundTaskDal backgroundTaskDal,
        CancellationToken cancellationToken)
    {
        var model = new CreateCommand
        {
            Text = backgroundTaskDal.Text,
            Delay = backgroundTaskDal.Delay
        };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Delete a background task by prism id
    /// </summary>
    /// <param name="text"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <returns>The returned value is the id from the database</returns>
    /// <response code="200">Object has been deleted</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpDelete("text/{text}")]
    [Authorization(resourcesNames: "Delete_Background_Tasks_By_Text")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteByText(string text, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new DeleteCommand
        {
            Text = text
        }, cancellationToken));
    }


    /// <summary>
    ///     Delete all background task
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <returns>The returned value is the id from the database</returns>
    /// <response code="200">Object has been deleted</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpDelete("delete/all")]
    [Authorization(resourcesNames: "Delete_Background_Tasks")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new DeleteAllCommand(), cancellationToken));
    }
}