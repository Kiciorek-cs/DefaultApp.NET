namespace Demo.BLL.Helpers.Regex;

internal static class EmailRegex
{
    public static string EmailFormatRegex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
}