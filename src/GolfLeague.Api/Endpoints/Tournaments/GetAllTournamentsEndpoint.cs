using GolfLeague.Api.Mapping;
using GolfLeague.Application.Models;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Endpoints.Tournaments;

public static class GetAllTournamentsEndpoint
{
    public const string Name = "GetAllTournaments";

    public static IEndpointRouteBuilder MapGetAllTournaments(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.Tournaments.GetAll, async (
            ITournamentService service,
            CancellationToken token = default) =>
        {
            IEnumerable<Tournament> tournament = await service.GetAllTournamentsAsync(token);
            var response = tournament.MapToResponse();
            return TypedResults.Ok(response);
        })
        .WithName(Name)
        .WithTags(GolfApiEndpoints.Tournaments.Tag)
        .Produces<TournamentsResponse>(contentType: "application/json");

        return app;
    }
}