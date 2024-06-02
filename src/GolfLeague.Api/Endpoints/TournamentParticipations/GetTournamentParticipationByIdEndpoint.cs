using GolfLeague.Api.Mapping;
using GolfLeague.Application.Models;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

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
                var request = new TournamentParticipation
                {
                    GolferId = golferId, TournamentId = tournamentId, Year = year
                };

                var tournamentParticipation = await service.GetTournamentParticipationById(
                    request,
                    cancellationToken);
                if (tournamentParticipation is null)
                {
                    return Results.Problem(statusCode: StatusCodes.Status404NotFound);
                }

                var response = tournamentParticipation.MapToResponse();
                return Results.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.TournamentParticipation.Tag)
            .Produces<TournamentParticipationResponse>(contentType: "application/json")
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound, contentType: "application/problem+json")
            .RequireAuthorization();

        return app;
    }
}