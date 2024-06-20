using GolfLeague.Api.Auth;
using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Endpoints.TournamentParticipations;

public static class UpdateGolferTournamentParticipationEndpoint
{
    private const string Name = "UpdateGolferTournamentParticipation";

    public static void MapUpdateGolferTournamentParticipation(this IEndpointRouteBuilder app)
    {
        app.MapPut(GolfApiEndpoints.Golfers.UpdateGolferTournamentParticipations, async (
                int id,
                UpdateGolferTournamentParticipationRequest request,
                ITournamentParticipationService service,
                CancellationToken token = default) =>
            {
                var update = request.MapToUpdateTournamentParticipation(id);
                var updatedParticipation = await service.UpdateAsync(update, token);

                if (updatedParticipation is null)
                {
                    return Results.Problem(statusCode: StatusCodes.Status404NotFound);
                }

                var response = updatedParticipation.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.TournamentParticipation.Tag)
            .Accepts<UpdateGolferTournamentParticipationRequest>(isOptional: false, contentType: "application/json")
            .Produces<TournamentParticipationResponse>(contentType: "application/json")
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound, contentType: "application/problem+json")
            .Produces<ValidationProblemDetails>(
                StatusCodes.Status400BadRequest,
                contentType: "application/problem+json")
            .RequireAuthorization(AuthConstants.TrustedPolicyName);
    }
}