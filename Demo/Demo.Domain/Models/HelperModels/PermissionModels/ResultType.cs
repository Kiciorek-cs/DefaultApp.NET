using Demo.Domain.Models.HelperModels.TokenModels;

namespace Demo.Domain.Models.HelperModels.PermissionModels;

public class ResultType
{
    public bool UnauthorizedResult { get; set; }
    public bool ForbidResult { get; set; }
    public TokenModel TokenData { get; set; } = new();
}