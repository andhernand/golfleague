using GolfLeague.Api.Auth;
using GolfLeague.Api.Endpoints.Filters;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Http.HttpResults;

namespace GolfLeague.Api.Endpoints.Tournaments;

public static class UpdateTournamentEndpoint
{
    private const string Name = "UpdateTournament";

    public static void MapUpdateTournament(this IEndpointRouteBuilder app)
    {
        app.MapPut(GolfApiEndpoints.Tournaments.Update,
                async Task<Results<Ok<TournamentResponse>, NotFound, ValidationProblem>> (
                    int id,
                    UpdateTournamentRequest request,
                    ITournamentService service,
                    CancellationToken token = default) =>
                {
                    var updatedTournament = await service.UpdateAsync(id, request, token);

                    return updatedTournament is null
                        ? TypedResults.NotFound()
                        : TypedResults.Ok(updatedTournament);
                })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .Accepts<UpdateTournamentRequest>(false, "application/json")
            .AddEndpointFilter<RequestValidationFilter<UpdateTournamentRequest>>()
            .RequireAuthorization(AuthConstants.TrustedPolicyName)
            .WithOpenApi();
    }
}