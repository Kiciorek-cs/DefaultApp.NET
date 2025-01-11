using System;
using System.Security.Cryptography;
using System.Text;

namespace Demo.BLL.Helpers.Permission.Password;

public class PasswordHashing
{
    private const int _size = 32;

    public static string GetNewGuid()
    {
        var guid = Guid.NewGuid();
        return guid.ToString();
    }

    //https://medium.com/@kefasogabi/implementing-authentication-and-authorization-in-asp-net-e831c04b4d38
    public static string GetUniqueSalt()
    {
        var letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var numbers = "1234567890";
        var specialChars = "!@#$%^&*()_+-=[]{};:'\"<>,.?/|~";

        var allowedChars = letters + numbers + specialChars;

        var chars = allowedChars.ToCharArray();

        var random = new Random();
        var salt = new char[_size];

        for (var i = 0; i < _size; i++) salt[i] = chars[random.Next(chars.Length)];

        var newCombination = new string(salt);

        var plainTextBytes = Encoding.UTF8.GetBytes(newCombination);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string HashUsingPbkdf2(string password, string salt)
    {
        using var bytes =
            new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 10000, HashAlgorithmName.SHA256);

        var derivedRandomKey = bytes.GetBytes(64);
        var hash = Convert.ToBase64String(derivedRandomKey);
        return hash;
    }
}