using System;
using Demo.Domain.Enums.Permission;

namespace Demo.Domain.Models.DALModels.Permission;

public class UpdateUserDal
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public GenderType Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string LoginName { get; set; }
}