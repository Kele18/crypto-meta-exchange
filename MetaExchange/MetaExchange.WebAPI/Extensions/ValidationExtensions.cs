using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace MetaExchange.WebAPI.Extensions;

public static class ValidationExtensions
{
    public static ValidationProblemDetails ToProblemDetails(this ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        return new ValidationProblemDetails(errors)
        {
            Title = "Validation Failed",
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };
    }
}