namespace Demo.BLL.Helpers.Regex;

internal static class IpAddressRegex
{
    public static string IpAddressFormatRegex = @"^((25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)(\.|$)){4}\b";
}