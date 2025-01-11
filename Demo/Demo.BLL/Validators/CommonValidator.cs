using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Demo.BLL.Helpers.Regex;
using FluentValidation;

namespace Demo.BLL.Validators;

public static class CommonValidator
{
    public static bool IsDateValid(string date)
    {
        return DateTime.TryParse(date, out _);
    }

    public static bool IsDateEarlierThanOtherDate(string dateFrom, string dateTo)
    {
        if (DateTime.TryParse(dateFrom, out var parsedDateFrom) && DateTime.TryParse(dateTo, out var parsedDateTo))
            return parsedDateFrom <= parsedDateTo;

        return false;
    }

    public static bool WhetherTheIdIsCorrect<T>(int? id, string propertyName, ValidationContext<T> context)
    {
        if (id <= 0)
        {
            context.AddCustomFailure($"{propertyName} must to greater then 0.", ValidationKeys.LowerThen);

            return false;
        }

        return true;
    }

    public static bool WhetherTheEnumIsCorrect<T, TEnum>(TEnum property, string propertyName,
        ValidationContext<T> context) where TEnum : struct, IConvertible
    {
        if (!Enum.IsDefined(typeof(TEnum), property))
        {
            context.AddCustomFailure($"{propertyName} does not have a valid value: {property}.",
                ValidationKeys.NotExist);

            return false;
        }

        return true;
    }

    public static bool WhetherTheStringValueIsValid<T>(string value, string propertyName,
        ValidationContext<T> context)
    {
        if (string.IsNullOrEmpty(value))
        {
            context.AddCustomFailure($"{propertyName} can't be null or empty.", ValidationKeys.NotEmpty);

            return false;
        }

        return true;
    }

    public static bool WhetherTheStringListValueAreValid<T>(List<string> values, string propertyName,
        ValidationContext<T> context)
    {
        if (!values.Any())
        {
            context.AddCustomFailure($"{propertyName} can't be empty.", ValidationKeys.NotEmpty);

            return false;
        }

        return true;
    }

    public static bool NullIntTimeoutIsValid<T>(int? value, string propertyName,
        ValidationContext<T> context)
    {
        if (value is not null && value < 1)
        {
            context.AddCustomFailure($"{propertyName} can't be lower then 1.", ValidationKeys.NullIntTimeout);

            return false;
        }

        return true;
    }

    public static bool EmailsChecker<T>(List<string> values,
        ValidationContext<T> context)
    {
        foreach (var value in values)
        {
            var isValid = EmailChecker(value, context);

            if (!isValid) return false;
        }

        return true;
    }

    public static bool EmailChecker<T>(string value,
        ValidationContext<T> context)
    {
        if (!Regex.IsMatch(value, EmailRegex.EmailFormatRegex))
        {
            context.AddCustomFailure($"Email format is not correct: {value}.", ValidationKeys.EmailIsNotCorrect);
            return false;
        }

        return true;
    }

    public static bool IpAddressChecker<T>(string value,
        ValidationContext<T> context)
    {
        if (!Regex.IsMatch(value, IpAddressRegex.IpAddressFormatRegex))
        {
            context.AddCustomFailure($"Ip address format is not correct: {value}.",
                ValidationKeys.IpAddressIsNotCorrect);
            return false;
        }

        return true;
    }

    public static bool DateOfBirthIsValid<T>(DateOnly dateOfBirth,
        ValidationContext<T> context)
    {
        if (dateOfBirth >= DateOnly.FromDateTime(DateTimeOffset.Now.DateTime))
        {
            context.AddCustomFailure($"The date cannot be later than the current one: {dateOfBirth}.",
                ValidationKeys.GreaterThan);
            return false;
        }

        return true;
    }

    public static bool PasswordChecker<T>(string value,
        ValidationContext<T> context)
    {
        if (!PasswordRegex.LowerChar.IsMatch(value))
        {
            context.AddCustomFailure("Password should contain at least one lower case letter",
                ValidationKeys.PasswordLowerChar);
            return false;
        }

        if (!PasswordRegex.UpperChar.IsMatch(value))
        {
            context.AddCustomFailure("Password should contain at least one upper case letter",
                ValidationKeys.PasswordUpperChar);
            return false;
        }

        if (!PasswordRegex.MiniMaxChars.IsMatch(value))
        {
            context.AddCustomFailure("Password should not be less than or greater than 12 characters",
                ValidationKeys.PasswordMiniMaxChars);
            return false;
        }

        if (!PasswordRegex.Number.IsMatch(value))
        {
            context.AddCustomFailure("Password should contain at least one numeric value",
                ValidationKeys.PasswordNumber);
            return false;
        }

        if (!PasswordRegex.Symbols.IsMatch(value))
        {
            context.AddCustomFailure("Password should contain at least one special case characters",
                ValidationKeys.PasswordSymbols);
            return false;
        }

        return true;
    }

    public static bool PasswordAndConfirmPasswordAreTheSame<T>(string password, string confirmPassword,
        ValidationContext<T> context)
    {
        if (password != confirmPassword)
        {
            context.AddCustomFailure("The passwords provided are not the same", ValidationKeys.NotEmpty,
                "ConfirmPassword");
            return false;
        }

        return true;
    }

    public static bool WhetherTheListIsEmpty<T, U>(List<U> values, string propertyName,
        ValidationContext<T> context)
    {
        if (!values.Any())
        {
            context.AddCustomFailure($"{propertyName} can't be empty.", ValidationKeys.NotEmpty);

            return false;
        }

        return true;
    }
}