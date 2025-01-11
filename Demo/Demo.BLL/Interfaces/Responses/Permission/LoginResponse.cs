using System;
using Demo.BLL.Interfaces.CQRS;

namespace Demo.BLL.Interfaces.Responses.Permission;

public class LoginResponse : BaseResponse, IResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }
    public int UserId { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
}