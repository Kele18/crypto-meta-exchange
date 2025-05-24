using Microsoft.AspNetCore.Mvc;

namespace MetaExchange.WebAPI.Extensions;

public static class ApiBehaviorExtensions
{
    public static IServiceCollection ConfigureCustomModelStateResponses(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var problemDetails = new ValidationProblemDetails(errors)
                {
                    Title = "Model Binding Failed",
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                };

                return new BadRequestObjectResult(problemDetails);
            };
        });

        return services;
    }
}