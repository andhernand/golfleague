using GolfLeague.Api.Auth;
using GolfLeague.Api.Endpoints.Filters;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Http.HttpResults;

namespace GolfLeague.Api.Endpoints.Tournaments;

public static class CreateTournamentEndpoint
{
    private const string Name = "CreateTournament";

    public static void MapCreateTournament(this IEndpointRouteBuilder app)
    {
        app.MapPost(GolfApiEndpoints.Tournaments.Create,
                async Task<Results<CreatedAtRoute<TournamentResponse>, ValidationProblem>> (
                    CreateTournamentRequest request,
                    ITournamentService service,
                    CancellationToken token = default) =>
                {
                    TournamentResponse tournament = await service.CreateAsync(request, token);

                    return TypedResults.CreatedAtRoute(
                        tournament,
                        GetTournamentByIdEndpoint.Name,
                        new { id = tournament.TournamentId });
                })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .Accepts<CreateTournamentRequest>(false, "application/json")
            .AddEndpointFilter<RequestValidationFilter<CreateTournamentRequest>>()
            .RequireAuthorization(AuthConstants.TrustedPolicyName)
            .WithOpenApi();
    }
}