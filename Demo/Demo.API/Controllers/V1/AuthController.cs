using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using Demo.API.Middlewares.Permission;
using Demo.BLL.Features.Auth.Commands.Login;
using Demo.BLL.Features.Auth.Commands.RefreshToken;
using Demo.BLL.Features.Auth.Commands.RevokeToken;
using Demo.BLL.Features.User.Commands.PasswordConfirmation;
using Demo.BLL.Features.User.Commands.PasswordReset;
using Demo.BLL.Interfaces.Responses.Common;
using Demo.BLL.Interfaces.Responses.Permission;
using Demo.Domain.Exceptions.Errors;
using Demo.Domain.Models.DALModels.Permission;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Demo.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : BaseController
{
    public AuthController(IMediator mediator) :
        base(mediator)
    {
    }

    /// <summary>
    ///     Login user
    /// </summary>
    /// <param name="login"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is login model</returns>
    /// <response code="201">Object has been created</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(LoginDal login,
        CancellationToken cancellationToken)
    {
        var model = new LoginCommand
        {
            EmailAddress = login.EmailAddress,
            Password = login.Password
        };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Create password change request
    /// </summary>
    /// <param name="resetPasswordDal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is user id</returns>
    /// <response code="201">Object has been created</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpPost("passwordResetRequest")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PasswordReset(ResetPasswordDal resetPasswordDal,
        CancellationToken cancellationToken)
    {
        var model = new PasswordResetCommand
        {
            EmailAddress = resetPasswordDal.EmailAddress
        };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Confirm password change
    /// </summary>
    /// <param name="confirmationPasswordDal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is user id</returns>
    /// <response code="201">Object has been created</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpPut("passwordResetConfirmation")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PasswordConfirmation(ConfirmationPasswordDal confirmationPasswordDal,
        CancellationToken cancellationToken)
    {
        var model = new PasswordConfirmationCommand
        {
            ResetToken = confirmationPasswordDal.ResetToken,
            Password = confirmationPasswordDal.Password,
            ConfirmPassword = confirmationPasswordDal.ConfirmPassword
        };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Refresh token
    /// </summary>
    /// <param name="tokenDal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is login model</returns>
    /// <response code="201">Object has been created</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpPut("refresh")]
    [Authorization(resourcesNames: "Refresh_Token")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshToken(TokenDal tokenDal,
        CancellationToken cancellationToken)
    {
        var model = new RefreshCommand
        {
            AccessToken = tokenDal.AccessToken,
            RefreshToken = tokenDal.RefreshToken
        };

        return Ok(await _mediator.Send(model, cancellationToken));
    }

    /// <summary>
    ///     Deactivate token
    /// </summary>
    /// <param name="tokenDal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>The returned value is user id</returns>
    /// <response code="201">Object has been created</response>
    /// <response code="400">Data is not valid</response>
    /// <response code="500">Exception</response>
    [HttpDelete("revoke")]
    [Authorization(resourcesNames: "Revoke_Token")]
    [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    //[Authorization]
    public async Task<IActionResult> RevokeToken(TokenRevokeModelDal tokenDal,
        CancellationToken cancellationToken)
    {
        var model = new RevokeCommand
        {
            AccessToken = tokenDal.AccessToken
        };

        return Ok(await _mediator.Send(model, cancellationToken));
    }
}