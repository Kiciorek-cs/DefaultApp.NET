using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using Demo.API.Middlewares.Permission;
using Demo.BLL.Features.Country.Queries.GetById;
using Demo.BLL.Features.Country.Queries.GetList;
using Demo.BLL.Features.Country.Queries.GetListPagination;
using Demo.BLL.Helpers.PagerExtension;
using Demo.Domain.Exceptions.Errors;
using Demo.Domain.Models.DTOModels.Country;
using Demo.Domain.Models.HelperModels.Pagination;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.API.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
//[Authorization(resourcesNames: "ADMIN")]
//[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
public class CountyController : BaseController
{
    public CountyController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    ///     Get all countries
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is a country list</returns>
    /// <response code="200">Return data</response>
    /// <response code="500">Exception</response>
    [HttpGet]
    [Authorization(resourcesNames: "Get_All_Country")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<CountryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetListQuery(), cancellationToken));
    }

    /// <summary>
    ///     Get countries with pagination
    /// </summary>
    /// <param name="pagination"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is a country list</returns>
    /// <response code="200">Return data</response>
    /// <response code="500">Exception</response>
    [HttpGet("pagination")]
    [Authorization(resourcesNames: "Get_All_Country")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(PaginationModelDto<CountryPaginationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetListPagination([FromQuery] UrlQueryPagination pagination,
        CancellationToken cancellationToken)
    {
        HttpContext.Items[PagerExtension.PaginationName] = pagination;

        return Ok(await _mediator.Send(new GetListPaginationQuery(), cancellationToken));
    }

    /// <summary>
    ///     Get one country by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is a country</returns>
    /// <response code="200">Return data</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpGet("{id:int}")]
    [Authorization(resourcesNames: "Get_Country_By_Id")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CountryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var model = new GetByIdQuery { Id = id };

        return Ok(await _mediator.Send(model, cancellationToken));
    }
}