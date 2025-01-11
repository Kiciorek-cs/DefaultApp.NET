namespace Demo.Domain.Models.HelperModels.TokenModels;

public class TokenModel
{
    public PermissionClaimModel PermissionClaimModels = new();

    public UserClaimModel UserModel = new();
}