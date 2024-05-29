using GolfLeague.Api.Mapping;
using GolfLeague.Application.Models;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

using SerilogTimings;

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
                using var timedOperation = Operation.Begin("Get ALl Tournaments");

                IEnumerable<Tournament> tournament = await service.GetAllTournamentsAsync(token);
                var response = tournament.MapToResponse();

                timedOperation.Complete();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .Produces<TournamentsResponse>(contentType: "application/json");

        return app;
    }
}