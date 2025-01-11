using System;
using System.Collections.Generic;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;

namespace Demo.BLL.Validators;

public class CustomValidator<T> : AbstractValidator<T>
{
    public CustomValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop; //important
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
    }

    public override ValidationResult Validate(ValidationContext<T> context)
    {
        var result = base.Validate(context);

        foreach (var error in result.Errors)
        {
            if (error.FormattedMessagePlaceholderValues == null)
                error.FormattedMessagePlaceholderValues = new Dictionary<string, object>();
            else if (error.FormattedMessagePlaceholderValues.ContainsKey("PropertyValue")) continue;

            var propertyValue = GetPropertyValue(context.InstanceToValidate,
                string.IsNullOrEmpty(context.PropertyName) ? error.PropertyName : context.PropertyName);

            if (string.IsNullOrEmpty(error.PropertyName)) error.PropertyName = context.PropertyName;

            error.FormattedMessagePlaceholderValues["PropertyValue"] = propertyValue;
        }

        return result;
    }

    private object GetPropertyValue(object obj, string propertyName)
    {
        if (propertyName is null) return string.Empty;

        var property = obj.GetType().GetProperty(propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property != null) return property.GetValue(obj);

        return string.Empty;
    }
}

public static class ValidationContextExtensions
{
    public static void AddCustomFailure<T>(this ValidationContext<T> context, string errorMessage, Guid keyValidator,
        string propertyName = null)
    {
        var failure = new CustomValidationFailure(propertyName, null, errorMessage, keyValidator);
        context.AddFailure(failure);
    }

    public static IRuleBuilderOptions<T, TProperty> WithCustomMessage<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, string errorMessage, Guid keyValidator)
    {
        return rule.WithMessage(errorMessage).WithErrorCode(keyValidator.GuidConversion())
            .WithState(_ => new CustomValidationFailure(null, null, errorMessage, keyValidator));
    }
}

public class CustomValidationFailure : ValidationFailure
{
    public CustomValidationFailure(string propertyName, object attemptedValue, string errorMessage, Guid keyValidator)
        : base(propertyName, errorMessage)
    {
        ErrorCode = keyValidator.GuidConversion();
        KeyValidator =
            keyValidator
                .ToString();
    }

    public string KeyValidator { get; }
}