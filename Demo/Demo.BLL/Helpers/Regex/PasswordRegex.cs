namespace Demo.BLL.Helpers.Regex;

internal static class PasswordRegex
{
    public static System.Text.RegularExpressions.Regex Number = new(@"[0-9]+");

    public static System.Text.RegularExpressions.Regex UpperChar = new(@"[A-Z]+");

    public static System.Text.RegularExpressions.Regex MiniMaxChars = new(@".{5,15}");

    public static System.Text.RegularExpressions.Regex LowerChar = new(@"[a-z]+");

    public static System.Text.RegularExpressions.Regex Symbols = new(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
}