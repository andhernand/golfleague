using GolfLeague.Api.Auth;
using GolfLeague.Api.Endpoints.Golfers;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Http.HttpResults;

namespace GolfLeague.Api.Endpoints.TournamentParticipation;

public static class CreateParticipationDetailEndpoint
{
    private const string Name = "CreateParticipationDetail";

    public static void MapParticipationDetailEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(GolfApiEndpoints.Golfers.CreateParticipationDetail,
                async Task<Results<CreatedAtRoute<TournamentParticipationResponse>, ValidationProblem>> (
                    int id,
                    ITournamentParticipationService service,
                    CreateParticipationDetailRequest request,
                    CancellationToken cancellationToken = default) =>
                {
                    var participation = await service.CreateAsync(id, request, cancellationToken);

                    return TypedResults.CreatedAtRoute(
                        participation,
                        GetGolferByIdEndpoint.Name,
                        new { id = participation?.GolferId });
                })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.TournamentParticipation.Tag)
            .RequireAuthorization(AuthConstants.TrustedPolicyName)
            .WithOpenApi();
    }
}