using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Helpers.Token;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.Domain.ConfigurationModels.DynamicModel;
using Demo.Domain.Entities.Permission;
using Demo.Domain.Enums;
using Demo.Domain.Enums.Permission;
using Demo.Domain.Models.HelperModels.PermissionModels;
using Demo.Domain.Models.HelperModels.TokenModels;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Demo.BLL.Services.TokenServices;

public class Token : IToken
{
    private readonly AuthorizeEnableSettings _authorizeEnabledSettings;
    private readonly IClock _clock;
    private readonly IDemoUnitOfWork _demoUnitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;
    private readonly JwtTokenSettings _settings;
    private readonly ITokenDataExtractor _tokenDataExtractor;

    public Token(
        IOptions<JwtTokenSettings> settings,
        IHttpContextAccessor httpContextAccessor,
        ITokenDataExtractor tokenDataExtractor,
        IOptions<AuthorizeEnableSettings> authorizeEnabledSettings,
        IPermissionUnitOfWork permissionUnitOfWork,
        IClock clock, IDemoUnitOfWork demoUnitOfWork)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenDataExtractor = tokenDataExtractor;
        _permissionUnitOfWork = permissionUnitOfWork;
        _clock = clock;
        _demoUnitOfWork = demoUnitOfWork;
        _authorizeEnabledSettings = authorizeEnabledSettings.Value;
        _settings = settings.Value;
    }

    /// <summary>
    /// </summary>
    /// <param name="token"></param>
    /// <param name="resources"></param>
    /// <param name="actionContext"></param>
    /// <param name="permissionType"></param>
    /// <param name="controllerType"></param>
    /// <param name="propertyName">the "propertyName" field tells you where to get the value to check permissions</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ResultType> AuthorizationUser(string token, IEnumerable<Claim> resources,
        AuthorizationFilterContext actionContext, PermissionType permissionType, ControllerType controllerType,
        string propertyName, CancellationToken cancellationToken = default)
    {
        if (!TokenHelper.ValidateParameters(_settings, token))
            return new ResultType { ForbidResult = false, UnauthorizedResult = true };

        try
        {
            var (permissions, user) = await ValidAndDecodeToken(token, cancellationToken);

            if (permissions is null || user is null)
                return new ResultType { ForbidResult = false, UnauthorizedResult = true };

            Log.Information("User with an {email} address is trying to log into the application {application}",
                user.EmailAddress, _settings.Application);

            var resultType = new ResultType
            {
                ForbidResult = false,
                UnauthorizedResult = false,
                TokenData = new TokenModel
                {
                    PermissionClaimModels = permissions,
                    UserModel = user
                }
            };

            var parameter = await GetParameterFromBody(actionContext, propertyName);
            if (parameter is null && !string.IsNullOrEmpty(propertyName))
                return ReturnForbidden(resultType);

            if (permissionType == PermissionType.User)
                if (user.UserId != parameter)
                    return ReturnForbidden(resultType);

            resultType.ForbidResult =
                !resources.All(x => permissions.Role.ClaimDetailModel.Resources.Contains(x.Value));

            return resultType;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception occurred during checking resources.");
            return new ResultType { ForbidResult = false, UnauthorizedResult = true };
        }
    }

    public (PermissionClaimModel permission, UserClaimModel user) GetTokenInformationFromHeader(
        CancellationToken cancellationToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = GetTokenFromHeader();

            if (string.IsNullOrEmpty(token)) return (null, new UserClaimModel());

            var jwtToken = tokenHandler.ReadJwtToken(token);

            var (permissionClaimModel, userClaimModel) =
                _tokenDataExtractor.DeserializerCustomClaims(jwtToken.Claims, cancellationToken);
            return (permissionClaimModel, userClaimModel);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception occurred during getting information.");
            return (null, null);
        }
    }

    public (PermissionClaimModel permission, UserClaimModel user) GetTokenInformation(string token,
        CancellationToken cancellationToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = tokenHandler.ReadJwtToken(token);

            var (permissionClaimModel, userClaimModel) =
                _tokenDataExtractor.DeserializerCustomClaims(jwtToken.Claims, cancellationToken);
            return (permissionClaimModel, userClaimModel);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception occurred during getting information.");
            return (null, null);
        }
    }

    public async Task<string> GenerateAccessToken(Login login, CancellationToken cancellationToken)
    {
        return await GenerateAccessToken(login.UserId, login.TokenGenerationTime, cancellationToken);
    }

    public async Task<string> GenerateConfirmationToken(Login login, DateTimeOffset expiresDate,
        CancellationToken cancellationToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.Secret);

        var userBody = await _tokenDataExtractor.CreateUserCustomSubjectForJwtToken(login.UserId, cancellationToken);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            //Claims = claims.ToDictionary(c => c.Type, c => (object)c.Value),
            Subject = new ClaimsIdentity(new[]
            {
                userBody
            }),
            Expires = expiresDate.DateTime,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken(CancellationToken cancellationToken)
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string GenerateConfirmationLink(int userId, string token)
    {
        return $"{_settings.Host}/User/accountConfirmation?UserId={userId}&ConfirmationToken={token}";
    }

    public string GeneratePasswordConfirmationLink(string token)
    {
        return $"{_settings.Host}/User/passwordResetRequest?PasswordConfirmationToken={token}";
    }


    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, CancellationToken cancellationToken)
    {
        var (principal, jwtToken) = ValidToken(token, cancellationToken);

        if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature,
                StringComparison.InvariantCultureIgnoreCase))
            throw new ValidationException("Invalid token");

        return principal;
    }

    public (ClaimsPrincipal validatedToken, JwtSecurityToken jwtToken) ValidToken(string token,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.Secret));

            var validate = tokenHandler.ValidateToken(token, SetTokenValidationParameters(symmetricSecurityKey),
                out var validatedToken);

            var jwtToken =
                (JwtSecurityToken)validatedToken;

            return (validate, jwtToken);
        }
        catch
        {
            throw new ValidationException("Invalid token");
        }
    }

    private ResultType ReturnForbidden(ResultType resultType)
    {
        resultType.ForbidResult = true;

        return resultType;
    }

    private async Task<(PermissionClaimModel permission, UserClaimModel user)> ValidAndDecodeToken(string token,
        CancellationToken cancellationToken)
    {
        try
        {
            var databaseToken =
                await _permissionUnitOfWork.TokenRepository.GetAsync(x => x.Value == token, cancellationToken);

            if (databaseToken is null || databaseToken.StatusType == TokenStatusType.Inactive) return (null, null);

            var (_, jwtToken) = ValidToken(token, cancellationToken);

            var (permissionClaimModel, userClaimModel) =
                _tokenDataExtractor.DeserializerCustomClaims(jwtToken.Claims, cancellationToken);

            return (permissionClaimModel, userClaimModel);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Exception occurred during decoded token.");
            return (null, null);
        }
    }

    private string GetTokenFromHeader()
    {
        return _httpContextAccessor.HttpContext?.Request.Headers["Bearer"];
    }

    private async Task<string> GenerateAccessToken(int userId, int liveTimeToken, CancellationToken cancellationToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_settings.Secret);

        var permissionBody =
            await _tokenDataExtractor.CreatePermissionCustomSubjectForJwtToken(userId, cancellationToken);
        var userBody = await _tokenDataExtractor.CreateUserCustomSubjectForJwtToken(userId, cancellationToken);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            Subject = new ClaimsIdentity(new[]
            {
                permissionBody,
                userBody
            }),
            Expires = _clock.Current().DateTime.AddMinutes(liveTimeToken),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private TokenValidationParameters SetTokenValidationParameters(SymmetricSecurityKey symmetricSecurityKey)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = symmetricSecurityKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience
        };
    }

    private async Task<int?> GetParameterFromBody(AuthorizationFilterContext actionContext, string propertyName)
    {
        int? paramValue = null;

        try
        {
            paramValue = GetParamValueFromQuery(actionContext.RouteData, propertyName);

            if (paramValue == null)
                paramValue = await GetParamValueFromBody(actionContext.HttpContext.Request, propertyName);
        }
        catch
        {
            paramValue = null;
        }

        return paramValue;
    }

    private int? GetParamValueFromQuery(RouteData queryParams, string paramName)
    {
        foreach (var queryParam in queryParams.Values)
            if (queryParam.Key.Equals(paramName, StringComparison.OrdinalIgnoreCase))
                return Convert.ToInt32(queryParam.Value);

        return null;
    }

    private async Task<int?> GetParamValueFromBody(HttpRequest request, string paramName)
    {
        request.EnableBuffering();

        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        var bodyJson = JsonSerializer.Deserialize<JsonElement>(body);

        try
        {
            foreach (var property in bodyJson.EnumerateObject())
                if (property.Name.Equals(paramName, StringComparison.OrdinalIgnoreCase))
                    if (property.Value.ValueKind == JsonValueKind.Number)
                        return property.Value.GetInt32();

            return null;
        }
        finally
        {
            request.Body.Position = 0;
        }
    }
}