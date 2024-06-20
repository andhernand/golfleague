using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Endpoints.Tournaments;

public static class GetAllTournamentsEndpoint
{
    private const string Name = "GetAllTournaments";

    public static void MapGetAllTournaments(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.Tournaments.GetAll, async (
                ITournamentService service,
                CancellationToken token = default) =>
            {
                var tournaments = await service.GetAllTournamentsAsync(token);
                var response = tournaments.MapToResponse();

                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .Produces<TournamentsResponse>(contentType: "application/json")
            .RequireAuthorization();
    }
}