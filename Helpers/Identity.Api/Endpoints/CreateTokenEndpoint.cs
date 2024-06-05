using Serilog;

namespace Identity.Api.Endpoints;

public static class CreateTokenEndpoint
{
    private const string Name = "CreateToken";
    private static readonly Serilog.ILogger Logger = Log.ForContext(typeof(CreateTokenEndpoint));

    public static IEndpointRouteBuilder MapCreateToken(this IEndpointRouteBuilder app)
    {
        app.MapPost(IdentityApiConstants.Tokens.Create, (TokenGenerationRequest request) =>
            {
                Logger.Information("Generating {@TokenGenerationRequest}", request);
                var jwt = JwtGenerator.GenerateToken(request.IsAdmin, request.IsTrusted);
                Logger.Verbose("Created: {JwtToken}", jwt);
                return TypedResults.Text(jwt);
            })
            .WithName(Name)
            .WithTags(IdentityApiConstants.Tokens.Tag)
            .Produces<string>(contentType: "text/plain")
            .Accepts<TokenGenerationRequest>(contentType: "application/json");

        return app;
    }
}