using GolfLeague.Api.Auth;
using GolfLeague.Api.Endpoints.Tournaments;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Http.HttpResults;

namespace GolfLeague.Api.Endpoints.TournamentParticipation;

public static class CreateTournamentDetailEndpoint
{
    private const string Name = "CreateTournamentGolferParticipation";

    public static void MapCreateTournamentDetailEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(GolfApiEndpoints.Tournaments.CreateTournamentDetail,
                async Task<Results<CreatedAtRoute<TournamentParticipationResponse>, ValidationProblem>> (
                    int id,
                    CreateTournamentDetailRequest request,
                    ITournamentParticipationService service,
                    CancellationToken cancellationToken = default) =>
                {
                    var participation = await service.CreateAsync(id, request, cancellationToken);

                    return TypedResults.CreatedAtRoute(
                        participation,
                        GetTournamentByIdEndpoint.Name,
                        new { id = participation?.TournamentId });
                })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.TournamentParticipation.Tag)
            .RequireAuthorization(AuthConstants.TrustedPolicyName)
            .WithOpenApi();
    }
}