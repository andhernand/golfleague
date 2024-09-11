using GolfLeague.Application.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Endpoints.Tournaments;

public static class GetTournamentByIdEndpoint
{
    public const string Name = "GetTournamentById";

    public static void MapGetTournamentById(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.Tournaments.Get, async (
                int id,
                ITournamentService service,
                CancellationToken token = default) =>
            {
                var tournament = await service.GetTournamentByIdAsync(id, token);
                if (tournament is null)
                {
                    return Results.Problem(statusCode: StatusCodes.Status404NotFound);
                }

                var response = tournament.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .Produces<TournamentResponse>(contentType: "application/json")
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound, contentType: "application/problem+json")
            .RequireAuthorization();
    }
}