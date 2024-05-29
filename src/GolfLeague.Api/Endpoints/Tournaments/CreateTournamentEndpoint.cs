using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using SerilogTimings;

namespace GolfLeague.Api.Endpoints.Tournaments;

public static class CreateTournamentEndpoint
{
    public const string Name = "CreateTournament";

    public static IEndpointRouteBuilder MapCreateTournament(this IEndpointRouteBuilder app)
    {
        app.MapPost(GolfApiEndpoints.Tournaments.Create, async (
                CreateTournamentRequest request,
                ITournamentService service,
                CancellationToken token = default) =>
            {
                using var timedOperation = Operation.Begin("Create Tournament");

                var tournament = request.MapToTournament();

                int createdId = await service.CreateAsync(tournament, token);
                tournament.TournamentId = createdId;

                var response = tournament.MapToResponse();

                timedOperation.Complete();
                return TypedResults.CreatedAtRoute(
                    response,
                    GetTournamentByIdEndpoint.Name,
                    new { id = tournament.TournamentId });
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .Accepts<CreateTournamentRequest>(false, "application/json")
            .Produces<TournamentResponse>(StatusCodes.Status201Created)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest);

        return app;
    }
}