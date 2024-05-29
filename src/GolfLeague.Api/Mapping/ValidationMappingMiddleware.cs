using FluentValidation;

using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Mapping;

public class ValidationMappingMiddleware(RequestDelegate next, ILogger<ValidationMappingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = 400;
            var validationFailureResponse = new ValidationFailureResponse
            {
                Errors = ex.Errors.Select(x => new ValidationResponse
                {
                    PropertyName = x.PropertyName, ErrorMessage = x.ErrorMessage
                })
            };

            logger.LogWarning("Validation failure: {@ValidationFailureResponse}", validationFailureResponse);

            await context.Response.WriteAsJsonAsync(validationFailureResponse);
        }
    }
}