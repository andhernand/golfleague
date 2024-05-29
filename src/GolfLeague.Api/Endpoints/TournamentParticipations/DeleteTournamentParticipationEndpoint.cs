using GolfLeague.Application.Models;
using GolfLeague.Application.Services;

using Microsoft.AspNetCore.Mvc;

using SerilogTimings;

namespace GolfLeague.Api.Endpoints.TournamentParticipations;

public static class DeleteTournamentParticipationEndpoint
{
    public const string Name = "DeleteTournamentParticipation";

    public static IEndpointRouteBuilder MapDeleteTournamentParticipation(this IEndpointRouteBuilder app)
    {
        app.MapDelete(GolfApiEndpoints.TournamentParticipation.Delete, async (
                [FromQuery] int golferId,
                [FromQuery] int tournamentId,
                [FromQuery] int year,
                ITournamentParticipationService service,
                CancellationToken cancellationToken = default) =>
            {
                using var timedOperation = Operation.Begin("Delete TournamentParticipation");

                var id = new TournamentParticipation { GolferId = golferId, TournamentId = tournamentId, Year = year };
                var deleted = await service.DeleteAsync(id, cancellationToken);

                timedOperation.Complete();
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.TournamentParticipation.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}