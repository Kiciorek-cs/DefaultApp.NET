using System;
using Demo.Domain.Enums.Permission;

namespace Demo.Domain.Models.DTOModels.User;

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string FirstNameUnified { get; set; }
    public string LastName { get; set; }
    public string LastNameUnified { get; set; }
    public GenderType Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string LoginName { get; set; }

    public string EmailAddress { get; set; }
    public int TokenGenerationTime { get; set; } = 30; // minutes
    public EmailValidationStatusType EmailValidationStatus { get; set; } = EmailValidationStatusType.NotConfirmed;
    public AccountStatusType Status { get; set; } = AccountStatusType.Suspended;
}