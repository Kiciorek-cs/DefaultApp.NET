using System;
using Demo.BLL.Interfaces.CQRS;
using Demo.Domain.Enums.Permission;
using MediatR;

namespace Demo.BLL.Features.User.Commands.Update;

public class UpdateCommand : IRequest<IResponse>
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public GenderType Gender { get; set; }

    public DateOnly DateOfBirth { get; set; }
    public string LoginName { get; set; }
}