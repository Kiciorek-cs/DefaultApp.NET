using Demo.Domain.Models.HelperModels.TokenModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.BLL.Interfaces.Services.TokenServices;

public interface ITokenDataExtractor
{
    Task<Claim> CreatePermissionCustomSubjectForJwtToken(int userId, CancellationToken cancellationToken);
    Task<Claim> CreateUserCustomSubjectForJwtToken(int userId, CancellationToken cancellationToken);

    (PermissionClaimModel permission, UserClaimModel user) DeserializerCustomClaims(IEnumerable<Claim> claims,
        CancellationToken cancellationToken);
}