using GolfLeague.Application.Services;

using SerilogTimings;

namespace GolfLeague.Api.Endpoints.Tournaments;

public static class DeleteTournamentEndpoint
{
    public const string Name = "DeleteTournament";

    public static IEndpointRouteBuilder MapDeleteTournament(this IEndpointRouteBuilder app)
    {
        app.MapDelete(GolfApiEndpoints.Tournaments.Delete, async (
                int id,
                ITournamentService service,
                CancellationToken token = default) =>
            {
                using var timedOperation = Operation.Begin("Delete Tournament");

                var deleted = await service.DeleteByIdAsync(id, token);

                timedOperation.Complete();
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}