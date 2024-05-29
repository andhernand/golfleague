using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

using SerilogTimings;

namespace GolfLeague.Api.Endpoints.Tournaments;

public static class GetTournamentByIdEndpoint
{
    public const string Name = "GetTournamentById";

    public static IEndpointRouteBuilder MapGetTournamentById(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.Tournaments.Get, async (
                int id,
                ITournamentService service,
                CancellationToken token = default) =>
            {
                using var timedOperation = Operation.Begin("Get Tournament By Id");

                var tournament = await service.GetTournamentByIdAsync(id, token);
                if (tournament is null)
                {
                    timedOperation.Complete();
                    return Results.NotFound();
                }

                var response = tournament.MapToResponse();

                timedOperation.Complete();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .Produces<TournamentResponse>(contentType: "application/json")
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}