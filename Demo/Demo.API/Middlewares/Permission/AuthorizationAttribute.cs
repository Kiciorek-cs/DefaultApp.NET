using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Demo.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Demo.API.Middlewares.Permission;

public class AuthorizationAttribute : TypeFilterAttribute
{
    private static readonly string _claimResourcesTypeName = "Resources";

    public AuthorizationAttribute(PermissionType permissionType = PermissionType.None,
        ControllerType controllerType = ControllerType.None, string propertyName = "",
        params string[] resourcesNames) : base(typeof(AuthorizeByResource))
    {
        Arguments = new object[]
            { permissionType, controllerType, propertyName, ConvertListResources(resourcesNames.ToList()) };
    }

    private IEnumerable<Claim> ConvertListResources(List<string> resourcesNames)
    {
        return resourcesNames.Select(x => new Claim(_claimResourcesTypeName, x));
    }
}