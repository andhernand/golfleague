using GolfLeague.Api.Auth;
using GolfLeague.Application.Services;

namespace GolfLeague.Api.Endpoints.Tournaments;

public static class DeleteTournamentEndpoint
{
    private const string Name = "DeleteTournament";

    public static IEndpointRouteBuilder MapDeleteTournament(this IEndpointRouteBuilder app)
    {
        app.MapDelete(GolfApiEndpoints.Tournaments.Delete, async (
                int id,
                ITournamentService service,
                CancellationToken token = default) =>
            {
                var deleted = await service.DeleteByIdAsync(id, token);
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.AdminPolicyName);

        return app;
    }
}