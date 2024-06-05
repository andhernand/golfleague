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

            var validationProblem = new ValidationProblemDetails(problems) { Status = StatusCodes.Status400BadRequest };

            _logger.Warning("Validation failure: {@ValidationProblemDetails}", validationProblem);

            await context.Response.WriteAsJsonAsync(validationProblem);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unhandled Exception Occured");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var problemDetail = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError, Title = ex.Message
            };

            await context.Response.WriteAsJsonAsync(problemDetail);
        }
    }
}