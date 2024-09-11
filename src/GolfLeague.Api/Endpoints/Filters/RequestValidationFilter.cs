using FluentValidation;

namespace GolfLeague.Api.Endpoints.Filters;

public class RequestValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<T>().First();
        var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);
        if (result.IsValid)
        {
            return await next(context);
        }

        return TypedResults.ValidationProblem(result.ToDictionary());
    }
}