using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Helpers.Permission;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.Domain.Enums;
using Demo.Domain.Enums.Permission;
using Demo.Domain.Models.HelperModels.TokenModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Demo.BLL.Services.TokenServices;

public class TokenDataExtractor : ITokenDataExtractor
{
    private readonly string _permissionName = "Permission";
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;
    private readonly string _userName = "User";

    public TokenDataExtractor(IPermissionUnitOfWork permissionUnitOfWork)
    {
        _permissionUnitOfWork = permissionUnitOfWork;
    }

    public async Task<Claim> CreatePermissionCustomSubjectForJwtToken(int userId, CancellationToken cancellationToken)
    {
        var user = await _permissionUnitOfWork.UserRepository.GetAsync(x => x.Id == userId, cancellationToken);

        var role = user.Role;

        var roleResourceNames = role.Resources.Select(y => y.Name).Distinct().ToList();

        var roleClaim = new RoleClaimModel
        {
            RoleName = role.Name,
            ClaimDetailModel = new SubjectClaimDetailModel
            {
                Resources = roleResourceNames
            }
        };

        var claims = new PermissionClaimModel
        {
            Role = roleClaim
        };

        return new Claim(_permissionName, JsonConvert.SerializeObject(claims), JsonClaimValueTypes.Json);
    }

    public async Task<Claim> CreateUserCustomSubjectForJwtToken(int userId, CancellationToken cancellationToken)
    {
        var user =
            await _permissionUnitOfWork.UserRepository.GetAsync(x => x.Id == userId, cancellationToken);

        var claims = new UserClaimModel
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailAddress = user.Login.EmailAddress,
            LoginName = user.Login.LoginName
        };

        return new Claim(_userName, JsonConvert.SerializeObject(claims), JsonClaimValueTypes.Json);
    }


    public (PermissionClaimModel permission, UserClaimModel user) DeserializerCustomClaims(IEnumerable<Claim> claims,
        CancellationToken cancellationToken)
    {
        var permissionClaimModels = new PermissionClaimModel();
        var userClaimModel = new UserClaimModel();

        var jsonOption = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        foreach (var claim in claims)
        {
            if (TokenClaimsType.Permisson.EnumToString() == claim.Type)
                permissionClaimModels = JsonSerializer.Deserialize<PermissionClaimModel>(claim.Value, jsonOption);

            if (TokenClaimsType.User.EnumToString() == claim.Type)
                userClaimModel = JsonSerializer.Deserialize<UserClaimModel>(claim.Value, jsonOption);
        }

        return (permissionClaimModels, userClaimModel);
    }
}