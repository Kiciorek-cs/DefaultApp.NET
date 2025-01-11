using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.CQRS;
using Demo.Domain.Exceptions.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Demo.API.Controllers;

public class BaseController : Controller
{
    public readonly IMediator _mediator;

    public BaseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> ModelValid<T>(T model)
    {
        TryValidateModel(model);

        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(model);

            if (response != null && response.GetType() != typeof(BaseResponse)) return Ok(response);

            if (response != null && ((BaseResponse)response).Successful)
                return Ok(response);

            return BadRequest(response);
        }

        var listErrors = ModelState.Values.Where(v => v.Errors.Count > 0).ToList();
        var errorList = new List<Error>();
        foreach (var error in listErrors)
        foreach (var err in error.Errors.ToList())
            errorList.Add(new Error
            {
                Key = error.GetType().GetProperty("Key")?.GetValue(error)?.ToString(),
                Value = err.ErrorMessage
            });

        return BadRequest(new
        {
            Message = errorList[0].Value,
            Errors = errorList,
            Successfull = false
        });
    }
}