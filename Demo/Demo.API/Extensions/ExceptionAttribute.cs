using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Demo.BLL.Services.ErrorLogger;
using Demo.BLL.Validators;
using Demo.Domain.Exceptions.Errors;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Demo.API.Extensions;
public static class ApplicationBuilderExtensions
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(x =>
        {
            x.Run(async context =>
            {
                var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = errorFeature.Error;

                if (exception is ValidationException validationException)
                    await context.ValidationExceptionMethod(validationException);
                else if (exception is ForbiddenError forbiddenError)
                    await context.ForbiddenExceptionMethod(forbiddenError);
                else
                    await context.GeneralExceptionMethod(exception);
            });
        });
    }

    private static async Task GeneralExceptionMethod(this HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var error = new ErrorDetails
        {
            StatusCode = context.Response.StatusCode
        };

        if (!string.IsNullOrEmpty(exception.Message))
            error.Message = exception.Message;

        ErrorLogger.LogError(
            DateTime.Now,
            $"Something was wrong: \n\tmethod name: {new StackTrace(exception).GetFrame(0)?.GetMethod()?.Name}, \n\tmessage: {error.Message}");

        Log.Error(
            $"Something was wrong: \n\tmethod name: {new StackTrace(exception).GetFrame(0)?.GetMethod()?.Name}, \n\tmessage: {error.Message} \n\t Time: {DateTime.Now}");

        await context.Response.WriteAsync(error.ToString());
    }

    private static async Task ValidationExceptionMethod(this HttpContext context,
        ValidationException validationException)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        string GetPropertyCurrentValue(ValidationFailure error)
        {
            try
            {
                error.FormattedMessagePlaceholderValues.TryGetValue("PropertyValue", out var value);

                return value is null ? string.Empty : value.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        string GetPropertyValidationKey(ValidationFailure error)
        {
            try
            {
                if (error.CustomState is CustomValidationFailure customFailureCustomState)
                    return customFailureCustomState.KeyValidator;
                if (error is CustomValidationFailure customFailure) return customFailure.KeyValidator;

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        var errors = new ValidationError
        {
            StatusCode = context.Response.StatusCode,
            Successfull = false,
            Errors = validationException.Errors.Select(err => new ValidationErrorDetails
            {
                Message = err.ErrorMessage,
                Error = err.ErrorCode,
                Property = err.PropertyName,
                AttemptedValue = GetPropertyCurrentValue(err),
                KeyValidator = GetPropertyValidationKey(err)
            })
        };

        await context.Response.WriteAsync(errors.ToString(), Encoding.UTF8);
    }

    private static async Task ForbiddenExceptionMethod(this HttpContext context, ForbiddenError forbiddenError)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(forbiddenError.Message);
    }
}