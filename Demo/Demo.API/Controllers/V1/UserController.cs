using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using Demo.API.Middlewares.Permission;
using Demo.BLL.Features.User.Commands.ConfirmAccount;
using Demo.BLL.Features.User.Commands.ConfirmAccountByUserId;
using Demo.BLL.Features.User.Commands.Create;
using Demo.BLL.Features.User.Commands.CreateByUser;
using Demo.BLL.Features.User.Commands.Delete;
using Demo.BLL.Features.User.Commands.SuspendAccount;
using Demo.BLL.Features.User.Commands.UnblockAccount;
using Demo.BLL.Features.User.Commands.Update;
using Demo.BLL.Features.User.Commands.UpdateRole;
using Demo.BLL.Features.User.Queries.GetById;
using Demo.BLL.Features.User.Queries.GetList;
using Demo.BLL.Features.User.Queries.GetOwnAccount;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.Domain.Enums;
using Demo.Domain.Exceptions.Errors;
using Demo.Domain.Models.DALModels.Permission;
using Demo.Domain.Models.DTOModels.User;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UserController : BaseController
{
    public UserController(IMediator mediator) :
        base(mediator)
    { }

    /// <summary>
    ///     Get all users
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is a user list</returns>
    /// <response code="200">Return data</response>
    /// <response code="500">Exception</response>
    [HttpGet]
    //[Authorization(resourcesNames: "Get_All_User")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetListQuery(), cancellationToken));
    }

    /// <summary>
    ///     Get one user by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is a user</returns>
    /// <response code="200">Return data</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpGet("{id:int}")]
    //[Authorization(resourcesNames: "Get_User_By_Id")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var model = new GetByIdQuery { Id = id };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Get own account
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is a user</returns>
    /// <response code="200">Return data</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpGet("own")]
    //[Authorization(resourcesNames: "Get_Own_User_Information")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOwnAccount(CancellationToken cancellationToken)
    {
        var model = new GetOwnAccountQuery();

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Crate a new user
    /// </summary>
    /// <param name="createUserDal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is the id from the database</returns>
    /// <response code="201">Object has been created</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateNewUser(CreateUserDal createUserDal,
        CancellationToken cancellationToken)
    {
        var model = new CreateCommand
        {
            FirstName = createUserDal.FirstName,
            LastName = createUserDal.LastName,
            Gender = createUserDal.Gender,
            DateOfBirth = createUserDal.DateOfBirth,
            EmailAddress = createUserDal.EmailAddress,
            LoginName = createUserDal.LoginName,
            Password = createUserDal.Password,
            ConfirmPassword = createUserDal.ConfirmPassword
        };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Crate a new user by user
    /// </summary>
    /// <param name="createUserDal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is the id from the database</returns>
    /// <response code="201">Object has been created</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpPost("registerBySomeone")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateNewUserByUser(CreateUserDal createUserDal,
        CancellationToken cancellationToken)
    {
        var model = new CreateByUserCommand
        {
            FirstName = createUserDal.FirstName,
            LastName = createUserDal.LastName,
            Gender = createUserDal.Gender,
            DateOfBirth = createUserDal.DateOfBirth,
            EmailAddress = createUserDal.EmailAddress,
            LoginName = createUserDal.LoginName,
            Password = createUserDal.Password,
            ConfirmPassword = createUserDal.ConfirmPassword
        };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Confirm account
    /// </summary>
    /// <param name="confirmationUserDal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is the id from the database</returns>
    /// <response code="201">Object has been created</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpGet("accountConfirmation")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConfirmAccount([FromQuery] ConfirmationUserDal confirmationUserDal,
        CancellationToken cancellationToken)
    {
        var model = new ConfirmAccountCommand
        {
            UserId = confirmationUserDal.UserId,
            ConfirmationToken = confirmationUserDal.ConfirmationToken
        };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Confirm account by user id
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is the id from the database</returns>
    /// <response code="201">Object has been created</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpGet("accountConfirmation/{userId}")]
    //[Authorization(resourcesNames: "Edit_Account_Confirmation_Status")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConfirmAccountByUserId(int userId,
        CancellationToken cancellationToken)
    {
        var model = new ConfirmAccountByUserIdCommand
        {
            UserId = userId
        };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Update user account
    /// </summary>
    /// <param name="updateUserDal"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <returns>The returned value is user id</returns>
    /// <response code="201">Object has been created</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpPut("{id:int}")]
    [Authorization(PermissionType.User, ControllerType.User, "id", "Edit_User")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, UpdateUserDal updateUserDal,
        CancellationToken cancellationToken)
    {
        var model = new UpdateCommand
        {
            Id = id,
            LoginName = updateUserDal.LoginName,
            LastName = updateUserDal.LastName,
            DateOfBirth = updateUserDal.DateOfBirth,
            FirstName = updateUserDal.FirstName,
            Gender = updateUserDal.Gender
        };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Update user role
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <returns>The returned value is user id</returns>
    /// <response code="201">Object has been created</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpPut("role/{id:int}")]
    //[Authorization(resourcesNames: "Edit_User_Role")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] int roleId,
        CancellationToken cancellationToken)
    {
        var model = new UpdateRoleCommand
        {
            Id = id,
            RoleId = roleId
        };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Suspend account
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <returns>The returned value is user id</returns>
    /// <response code="201">Object has been updated</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpPut("{id:int}/suspend")]
    [Authorization(resourcesNames: "Suspend_User")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SuspendAccount(int id,
        CancellationToken cancellationToken)
    {
        var model = new SuspendAccountCommand { Id = id };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Unblock account
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <returns>The returned value is user id</returns>
    /// <response code="201">Object has been updated</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpPut("{id:int}/unblock")]
    [Authorization(resourcesNames: "Unblock_User")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UnblockAccount(int id,
        CancellationToken cancellationToken)
    {
        var model = new UnblockAccountCommand { Id = id };

        return Ok(await _mediator.Send(model, cancellationToken));
    }


    /// <summary>
    ///     Delete user account
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is user id</returns>
    /// <response code="201">Object has been created</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpDelete("{id:int}")]
    [Authorization(PermissionType.User, ControllerType.User, "id", "Delete_User")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id,
        CancellationToken cancellationToken)
    {
        var model = new DeleteCommand
        {
            Id = id
        };

        return Ok(await _mediator.Send(model, cancellationToken));
    }
}