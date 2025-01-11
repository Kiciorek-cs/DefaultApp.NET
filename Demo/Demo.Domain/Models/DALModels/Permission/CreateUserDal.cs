using System;
using Demo.Domain.Enums.Permission;

namespace Demo.Domain.Models.DALModels.Permission;

public class CreateUserDal
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