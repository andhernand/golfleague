using FluentValidation;

using Microsoft.AspNetCore.Mvc;

using Serilog;

namespace GolfLeague.Api.Mapping;

public class ValidationMappingMiddleware(RequestDelegate next)
{
    private readonly Serilog.ILogger _logger = Log.ForContext<ValidationMappingMiddleware>();

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var problems = ex.Errors.GroupBy(error => error.PropertyName)
                .ToDictionary(group => group.Key, group => group
                    .Select(error => error.ErrorMessage)
                    .ToArray());

            var validationProblem = new ValidationProblemDetails(problems)
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Status = StatusCodes.Status400BadRequest
            };

            _logger.Warning("Validation failure: {@ValidationProblemDetails}", validationProblem);

            await context.Response.WriteAsJsonAsync(validationProblem);
        }
    }
}