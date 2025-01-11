using System.Collections.Generic;
using Demo.Domain.Enums.Permission;

namespace Demo.Domain.Entities.Permission;

public class Login
{
    public int Id { get; set; }
    public string LoginName { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public string EmailAddress { get; set; }
    public int TokenGenerationTime { get; set; } = 30; // minutes
    public EmailValidationStatusType EmailValidationStatus { get; set; } = EmailValidationStatusType.NotConfirmed;
    public AccountStatusType Status { get; set; } = AccountStatusType.Suspended;

    public int UserId { get; set; }
    public virtual User User { get; set; }

    public virtual ICollection<Token> Tokens { get; set; }
}