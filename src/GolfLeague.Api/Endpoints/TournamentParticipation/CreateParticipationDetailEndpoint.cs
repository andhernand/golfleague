using GolfLeague.Api.Auth;
using GolfLeague.Api.Endpoints.Golfers;
using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Endpoints.TournamentParticipation;

public static class CreateParticipationDetailEndpoint
{
    private const string Name = "CreateParticipationDetail";

    public static void MapParticipationDetailEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(GolfApiEndpoints.Golfers.CreateParticipationDetail, async (
                int id,
                ITournamentParticipationService service,
                CreateParticipationDetailRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var participation = request.MapToTournamentParticipation(id);
                _ = await service.CreateAsync(participation, cancellationToken);

                var response = participation.MapToResponse();
                return TypedResults.CreatedAtRoute(
                    response,
                    GetGolferByIdEndpoint.Name,
                    new { id = response.GolferId });
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.TournamentParticipation.Tag)
            .Produces<TournamentParticipationResponse>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(
                StatusCodes.Status400BadRequest,
                contentType: "application/problem+json")
            .RequireAuthorization(AuthConstants.TrustedPolicyName);
    }
}