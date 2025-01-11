using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Demo.Domain.Entities.Permission;
using Demo.Domain.Enums;
using Demo.Domain.Models.HelperModels.PermissionModels;
using Demo.Domain.Models.HelperModels.TokenModels;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Demo.BLL.Interfaces.Services.TokenServices;

public interface IToken
{
    Task<ResultType> AuthorizationUser(string token, IEnumerable<Claim> resources,
        AuthorizationFilterContext actionContext, PermissionType permissionType, ControllerType controllerType,
        string propertyName, CancellationToken cancellationToken = default);
    Task<string> GenerateConfirmationToken(Login login, DateTimeOffset dateTimeOffset,
        CancellationToken cancellationToken);
    Task<string> GenerateAccessToken(Login login, CancellationToken cancellationToken);
    string GenerateRefreshToken(CancellationToken cancellationToken);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token, CancellationToken cancellationToken);
    (ClaimsPrincipal validatedToken, JwtSecurityToken jwtToken) ValidToken(string token,
        CancellationToken cancellationToken = default);
    string GenerateConfirmationLink(int userId, string token);
    string GeneratePasswordConfirmationLink(string token);
    (PermissionClaimModel permission, UserClaimModel user) GetTokenInformationFromHeader(
        CancellationToken cancellationToken);
    (PermissionClaimModel permission, UserClaimModel user) GetTokenInformation(string token,
        CancellationToken cancellationToken);
}