using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using Demo.API.Middlewares.Permission;
using Demo.BLL.Features.Resource.Commands.Create;
using Demo.BLL.Features.Resource.Commands.Delete;
using Demo.BLL.Features.Resource.Commands.Update;
using Demo.BLL.Features.Resource.Queries.GetById;
using Demo.BLL.Features.Resource.Queries.GetList;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.Domain.Exceptions.Errors;
using Demo.Domain.Models.DALModels.Resource;
using Demo.Domain.Models.DTOModels.Resource;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ResourceController : BaseController
{
    public ResourceController(IMediator mediator) :
        base(mediator)
    {
    }

    /// <summary>
    ///     Get all resources
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is a resource list</returns>
    /// <response code="200">Return data</response>
    /// <response code="500">Exception</response>
    [HttpGet]
    //[Authorization(resourcesNames: "Get_All_Resource")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<ResourceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetListQuery(), cancellationToken));
    }

    /// <summary>
    ///     Get one resource by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is a resource</returns>
    /// <response code="200">Return data</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpGet("{id:int}")]
    //[Authorization(resourcesNames: "Get_Resource_By_Id")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResourceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var model = new GetByIdQuery { Id = id };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Insert a resource
    /// </summary>
    /// <param name="resourceDal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is the id from the database</returns>
    /// <response code="201">Object has been created</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpPost]
    //[Authorization(resourcesNames: "Create_Resource")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(ResourceDal resourceDal,
        CancellationToken cancellationToken)
    {
        var model = new CreateCommand
        {
            Name = resourceDal.Name,
            Description = resourceDal.Description,
            HttpMethod = resourceDal.HttpMethod
        };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Update a resource by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="resourceDal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is the id from the database</returns>
    /// <response code="200">Object has been updated</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpPut("{id:int}")]
    //[Authorization(resourcesNames: "Edit_Resource")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, ResourceDal resourceDal,
        CancellationToken cancellationToken)
    {
        var model = new UpdateCommand
        {
            Id = id,
            Name = resourceDal.Name,
            Description = resourceDal.Description,
            HttpMethod = resourceDal.HttpMethod
        };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Delete a resource by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <returns>The returned value is the id from the database</returns>
    /// <response code="200">Object has been deleted</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpDelete("{id:int}")]
    //[Authorization(resourcesNames: "Delete_Resource")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new DeleteCommand { Id = id }, cancellationToken));
    }
}