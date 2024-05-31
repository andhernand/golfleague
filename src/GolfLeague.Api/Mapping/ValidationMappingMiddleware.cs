using FluentValidation;

using GolfLeague.Contracts.Responses;

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
            context.Response.StatusCode = 400;
            var validationFailureResponse = new ValidationFailureResponse
            {
                Errors = ex.Errors.Select(x => new ValidationResponse
                {
                    PropertyName = x.PropertyName, ErrorMessage = x.ErrorMessage
                })
            };

            _logger.Warning("Validation failure: {@ValidationFailureResponse}", validationFailureResponse);

            await context.Response.WriteAsJsonAsync(validationFailureResponse);
        }
    }
}