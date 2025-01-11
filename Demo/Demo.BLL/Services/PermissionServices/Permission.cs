using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Demo.BLL.Features.User.Commands.Create;
using Demo.BLL.Features.User.Commands.CreateByUser;
using Demo.BLL.Helpers.Permission.Password;
using Demo.BLL.Interfaces.Repositories;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Interfaces.Services.PermissionServices;
using Demo.BLL.Interfaces.Services.TokenServices;
using Demo.Domain.Entities.Permission;
using Demo.Domain.Enums.Permission;

namespace Demo.BLL.Services.PermissionServices;

public class Permission : IPermission
{
    private readonly IClock _clock;
    private readonly IMapper _mapper;
    private readonly IPermissionUnitOfWork _permissionUnitOfWork;
    private readonly IToken _token;

    public Permission(IMapper mapper, IPermissionUnitOfWork permissionUnitOfWork, IClock clock, IToken token)
    {
        _mapper = mapper;
        _permissionUnitOfWork = permissionUnitOfWork;
        _clock = clock;
        _token = token;
    }

    public async Task<User> AddUser<T>(T request, CancellationToken cancellationToken)
    {
        var defaultRole = await GetDefaultRole();

        var account = _mapper.Map<User>(request);
        account.RoleId = defaultRole.Id;

        var addedAccount = await _permissionUnitOfWork.UserRepository.AddAsync(account, cancellationToken);
        await _permissionUnitOfWork.CommitAsync(cancellationToken);

        return addedAccount;
    }

    public async Task<Login> AddLogin(CreateCommand request, User account, CancellationToken cancellationToken)
    {
        var sSalt = PasswordHashing.GetUniqueSalt();
        var sHashedPassword = PasswordHashing.HashUsingPbkdf2(request.Password, sSalt);

        var login = new Login
        {
            UserId = account.Id,
            LoginName = request.LoginName,
            PasswordHash = sHashedPassword,
            PasswordSalt = sSalt,
            EmailAddress = request.EmailAddress
        };

        var addedLogin = await _permissionUnitOfWork.LoginRepository.AddAsync(login, cancellationToken);
        await _permissionUnitOfWork.CommitAsync(cancellationToken);

        return addedLogin;
    }

    public async Task<Login> AddLogin(CreateByUserCommand request, User account, CancellationToken cancellationToken)
    {
        var sSalt = PasswordHashing.GetUniqueSalt();
        var sHashedPassword = PasswordHashing.HashUsingPbkdf2(request.Password, sSalt);

        var login = new Login
        {
            UserId = account.Id,
            LoginName = request.LoginName,
            PasswordHash = sHashedPassword,
            PasswordSalt = sSalt,
            EmailAddress = request.EmailAddress
        };

        var addedLogin = await _permissionUnitOfWork.LoginRepository.AddAsync(login, cancellationToken);
        await _permissionUnitOfWork.CommitAsync(cancellationToken);

        return addedLogin;
    }

    public async Task<Token> AddToken(Login login, DateTimeOffset expirationDate, string value, TokenType type,
        CancellationToken cancellationToken, int? parentId = null)
    {
        var jwtToken = new Token
        {
            LoginId = login.Id,
            CreatedAt = _clock.Current(),
            TokenType = type,
            StatusType = TokenStatusType.Active,
            ExpirationDate = expirationDate,
            Value = value,
            ParentTokenId = parentId
        };
        var addedToken = await _permissionUnitOfWork.TokenRepository.AddAsync(jwtToken, cancellationToken);
        await _permissionUnitOfWork.CommitAsync(cancellationToken);

        return addedToken;
    }

    public async Task<Token> AddConfirmationToken(User user, Login login, TokenType type,
        CancellationToken cancellationToken)
    {
        var expirationDate = _clock.Current().AddDays(1);
        var token = await _token.GenerateConfirmationToken(user.Login, expirationDate, cancellationToken);

        var confirmationToken = new Token
        {
            LoginId = login.Id,
            CreatedAt = _clock.Current(),
            TokenType = type,
            StatusType = TokenStatusType.Active,
            ExpirationDate = expirationDate,
            Value = token
        };

        var addedToken = await _permissionUnitOfWork.TokenRepository.AddAsync(confirmationToken, cancellationToken);
        await _permissionUnitOfWork.CommitAsync(cancellationToken);

        return addedToken;
    }

    private async Task<Role> GetDefaultRole()
    {
        return await _permissionUnitOfWork.RoleRepository.GetAsync(x => x.IsDefaultRole);
    }
}