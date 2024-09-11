using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Http.HttpResults;

namespace GolfLeague.Api.Endpoints.Tournaments;

public static class GetTournamentByIdEndpoint
{
    public const string Name = "GetTournamentById";

    public static void MapGetTournamentById(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.Tournaments.Get,
                async Task<Results<Ok<TournamentResponse>, NotFound>> (
                    int id,
                    ITournamentService service,
                    CancellationToken token = default) =>
                {
                    var tournament = await service.GetTournamentByIdAsync(id, token);

                    return tournament is null
                        ? TypedResults.NotFound()
                        : TypedResults.Ok(tournament);
                })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .RequireAuthorization()
            .WithOpenApi();
    }
}