using Demo.Domain.ConfigurationModels.DynamicModel;
using Serilog;

namespace Demo.BLL.Helpers.Token;

internal static class TokenHelper
{
    public static bool ValidateParameters(JwtTokenSettings settings, string token)
    {
        return ValidatorToken(token) && ValidatorAppSettings(settings);
    }

    private static bool ValidatorToken(string token)
    {
        if (token == null)
        {
            Log.Error("Not exists token in the request header.");
            return false;
        }

        return true;
    }

    private static bool ValidatorAppSettings(JwtTokenSettings settings)
    {
        if (settings is null || settings.Secret is null || settings.Application is null)
        {
            Log.Warning("Cannot find secret key for decode.");
            return false;
        }

        return true;
    }
}