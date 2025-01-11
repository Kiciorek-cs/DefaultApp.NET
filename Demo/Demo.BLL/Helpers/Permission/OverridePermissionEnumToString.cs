using Demo.Domain.Enums.Permission;

namespace Demo.BLL.Helpers.Permission;

public static class OverridePermissionEnumToString
{
    public static string EnumToString<T>(this T tokenClaims) where T : struct
    {
        return tokenClaims switch
        {
            TokenClaimsType.Permisson => "Permission",
            TokenClaimsType.User => "User",
            _ => "exp"
        };
    }
}