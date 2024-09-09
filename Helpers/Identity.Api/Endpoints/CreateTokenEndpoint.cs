using Serilog;

namespace Identity.Api.Endpoints;

public static class CreateTokenEndpoint
{
    private const string Name = "CreateToken";
    private static readonly Serilog.ILogger Logger = Log.ForContext(typeof(CreateTokenEndpoint));

    public static void MapCreateToken(this IEndpointRouteBuilder app)
    {
        app.MapPost(IdentityApiConstants.Tokens.Create, (TokenGenerationRequest request) =>
            {
                Logger.Information("Creating a JWT with the following Claims: {@TokenGenerationRequest}", request);

                var jwt = JwtGenerator.GenerateToken(request.IsAdmin, request.IsTrusted);
                return TypedResults.Text(jwt);
            })
            .WithName(Name)
            .WithTags(IdentityApiConstants.Tokens.Tag)
            .Accepts<TokenGenerationRequest>(contentType: "application/json")
            .Produces<string>(contentType: "application/jwt")
            .WithOpenApi();
    }
}