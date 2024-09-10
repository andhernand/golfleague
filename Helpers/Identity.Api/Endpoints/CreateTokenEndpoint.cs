using Identity.Api.Requests;
using Identity.Api.Services;

using Serilog;

namespace Identity.Api.Endpoints;

public static class CreateTokenEndpoint
{
    private const string Name = "CreateToken";
    private static readonly Serilog.ILogger Logger = Log.ForContext(typeof(CreateTokenEndpoint));

    public static void MapCreateToken(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/tokens", (
                TokenGenerationRequest request,
                JwtService service) =>
            {
                Logger.Information("Creating a JWT with the following Claims: {@TokenGenerationRequest}", request);

                var jwt = service.GenerateToken(request.IsAdmin, request.IsTrusted);
                return TypedResults.Text(jwt);
            })
            .WithName(Name)
            .WithTags("Tokens")
            .Accepts<TokenGenerationRequest>(contentType: "application/json")
            .Produces<string>(contentType: "application/jwt")
            .WithOpenApi();
    }
}