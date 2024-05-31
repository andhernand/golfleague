using GolfLeague.Api.Auth;
using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Endpoints.Tournaments;

public static class UpdateTournamentEndpoint
{
    private const string Name = "UpdateTournament";

    public static IEndpointRouteBuilder MapUpdateTournament(this IEndpointRouteBuilder app)
    {
        app.MapPut(GolfApiEndpoints.Tournaments.Update, async (
                int id,
                UpdateTournamentRequest request,
                ITournamentService service,
                CancellationToken token = default) =>
            {
                var tournament = request.MapToTournament(id);
                var updatedTournament = await service.UpdateAsync(tournament, token);
                if (updatedTournament is null)
                {
                    return Results.NotFound();
                }

                var response = updatedTournament.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .Accepts<UpdateTournamentRequest>(false, "application/json")
            .Produces<TournamentResponse>(contentType: "application/json")
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(AuthConstants.TrustedPolicyName);

        return app;
    }
}