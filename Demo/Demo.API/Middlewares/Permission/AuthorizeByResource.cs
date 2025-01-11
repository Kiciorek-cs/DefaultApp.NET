using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.Domain.ConfigurationModels.DynamicModel;
using Demo.Domain.Enums;
using Demo.Domain.Exceptions.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Serilog;

namespace Demo.API.Middlewares.Permission;

public class AuthorizeByResource : IAsyncAuthorizationFilter
{
    private readonly AuthorizeEnableSettings _authorizeEnabledSettings;
    private readonly ControllerType _controllerType;

    private readonly JwtTokenSettings _jwtSettings;
    private readonly PermissionType _permissionType;
    private readonly string _propertyName;
    private readonly IEnumerable<Claim> _resources;
    private readonly IToken _token;


    public AuthorizeByResource(
        PermissionType permissionType,
        ControllerType controllerType,
        string propertyName,
        IEnumerable<Claim> resources,
        IOptions<JwtTokenSettings> settings,
        IOptions<AuthorizeEnableSettings> authorizeEnabledSettings,
        IToken token
    )
    {
        _token = token;
        _controllerType = controllerType;
        _permissionType = permissionType;
        _propertyName = propertyName;
        _resources = resources;
        _jwtSettings = settings.Value;
        _authorizeEnabledSettings = authorizeEnabledSettings.Value;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext actionContext)
    {
        try
        {
            if (!_authorizeEnabledSettings.Authorize)
            {
                Log.Warning("Authorization disabled!");
                return;
            }

            var userContext = actionContext.HttpContext.Request;
            string token = userContext.Headers[_jwtSettings.HeaderName];

            var resultModel = await _token.AuthorizationUser(token, _resources, actionContext, _permissionType,
                _controllerType, _propertyName);

            if (_authorizeEnabledSettings.DeploymentMode)
            {
                Log.Warning("Deployment mode enabled!");
                var user = resultModel.TokenData.UserModel;
                if (!_authorizeEnabledSettings.DeploymentModeLogins.Contains(user.EmailAddress))
                    actionContext.Result = new UnauthorizedObjectResultError();
            }

            if (resultModel.UnauthorizedResult)
                actionContext.Result = new UnauthorizedObjectResultError();
            else if (resultModel.ForbidResult)
                actionContext.Result = new ForbiddenObjectResultError();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception occurred during authorization.");
            actionContext.Result = new UnauthorizedObjectResultError();
        }
    }
}