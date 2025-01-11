using System;
using System.Collections.Generic;
using Demo.Domain.Enums.Permission;

namespace Demo.Domain.Entities.Permission;

public class Token
{
    public int Id { get; set; }
    public TokenType TokenType { get; set; }
    public TokenStatusType StatusType { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
    public string Value { get; set; }

    public int LoginId { get; set; }
    public virtual Login Login { get; set; }

    public int? ParentTokenId { get; set; }
    public virtual Token ParentToken { get; set; }
    public virtual ICollection<Token> SubTokens { get; set; }
}