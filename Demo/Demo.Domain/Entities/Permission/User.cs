using System;
using System.Text.Json.Serialization;
using Demo.Domain.Enums.Permission;

namespace Demo.Domain.Entities.Permission;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string FirstNameUnified { get; set; }
    public string LastName { get; set; }
    public string LastNameUnified { get; set; }
    public GenderType Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public int RoleId { get; set; }

    [JsonIgnore] public virtual Login Login { get; set; }

    [JsonIgnore] public virtual Role Role { get; set; }
}