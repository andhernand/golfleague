using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Http.HttpResults;

namespace GolfLeague.Api.Endpoints.Tournaments;

public static class GetAllTournamentsEndpoint
{
    private const string Name = "GetAllTournaments";

    public static void MapGetAllTournaments(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.Tournaments.GetAll,
                async Task<Ok<IEnumerable<TournamentResponse>>> (
                    ITournamentService service,
                    CancellationToken token = default) =>
                {
                    var tournaments = await service.GetAllTournamentsAsync(token);
                    return TypedResults.Ok(tournaments);
                })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .RequireAuthorization()
            .WithOpenApi();
    }
}