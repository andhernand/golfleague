using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

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
                var tournament = await service.GetTournamentByIdAsync(id, token);
                if (tournament is null)
                {
                    return Results.NotFound();
                }

                var response = tournament.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .Produces<TournamentResponse>(contentType: "application/json")
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        return app;
    }
}