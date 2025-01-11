using System;
using System.Threading;
using System.Threading.Tasks;
using Demo.BLL.Features.User.Commands.Create;
using Demo.BLL.Features.User.Commands.CreateByUser;
using Demo.Domain.Entities.Permission;
using Demo.Domain.Enums.Permission;

namespace Demo.BLL.Interfaces.Services.PermissionServices;

public interface IPermission
{
    Task<User> AddUser<T>(T request, CancellationToken cancellationToken);
    Task<Login> AddLogin(CreateCommand request, User account, CancellationToken cancellationToken);
    Task<Login> AddLogin(CreateByUserCommand request, User account, CancellationToken cancellationToken);
    Task<Token> AddConfirmationToken(User user, Login login, TokenType type, CancellationToken cancellationToken);
    Task<Token> AddToken(Login login, DateTimeOffset expirationDate, string value, TokenType type,
        CancellationToken cancellationToken, int? parentId = null);
}