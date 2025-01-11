using System;
using Demo.BLL.Interfaces.CQRS;
using Demo.Domain.Enums.Permission;
using MediatR;

namespace Demo.BLL.Features.User.Commands.Create;

public class CreateCommand : IRequest<IResponse>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public GenderType Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string EmailAddress { get; set; }
    public string LoginName { get; set; }
    public string Password { get; set; }

    public string ConfirmPassword { get; set; }
}