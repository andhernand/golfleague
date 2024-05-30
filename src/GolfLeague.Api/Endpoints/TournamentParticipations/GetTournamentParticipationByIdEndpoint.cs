using GolfLeague.Api.Mapping;
using GolfLeague.Application.Models;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

using SerilogTimings;

namespace GolfLeague.Api.Endpoints.TournamentParticipations;

public static class GetTournamentParticipationByIdEndpoint
{
    public const string Name = "GetTournamentParticipationById";

    public static IEndpointRouteBuilder MapGetTournamentParticipationById(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.TournamentParticipation.GetById, async (
                [FromQuery] int golferId,
                [FromQuery] int tournamentId,
                [FromQuery] int year,
                ITournamentParticipationService service,
                CancellationToken cancellationToken = default) =>
            {
                using var timedOperation = Operation.Begin("Get TournamentParticipation By Id");

                var request = new TournamentParticipation
                {
                    GolferId = golferId, TournamentId = tournamentId, Year = year
                };

                var tournamentParticipation = await service.GetTournamentParticipationById(
                    request,
                    cancellationToken);
                if (tournamentParticipation is null)
                {
                    timedOperation.Complete();
                    return Results.NotFound();
                }

                var response = tournamentParticipation.MapToResponse();

                timedOperation.Complete();
                return Results.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.TournamentParticipation.Tag)
            .Produces<TournamentParticipationResponse>(contentType: "application/json")
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        return app;
    }
}