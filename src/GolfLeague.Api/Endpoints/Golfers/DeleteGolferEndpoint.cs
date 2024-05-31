using GolfLeague.Api.Auth;
using GolfLeague.Application.Services;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class DeleteGolferEndpoint
{
    private const string Name = "DeleteGolfer";

    public static IEndpointRouteBuilder MapDeleteGolfer(this IEndpointRouteBuilder app)
    {
        app.MapDelete(GolfApiEndpoints.Golfers.Delete, async (
                int id,
                IGolferService service,
                CancellationToken token = default) =>
            {
                var deleted = await service.DeleteByIdAsync(id, token);
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.AdminPolicyName);

        return app;
    }
}