using GolfLeague.Api.Auth;
using GolfLeague.Api.Endpoints.Tournaments;
using GolfLeague.Application.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Endpoints.TournamentParticipation;

public static class CreateTournamentDetailEndpoint
{
    private const string Name = "CreateTournamentGolferParticipation";

    public static void MapCreateTournamentDetailEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(GolfApiEndpoints.Tournaments.CreateTournamentDetail, async (
                int id,
                ITournamentParticipationService service,
                CreateTournamentDetailRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var participation = request.MapToTournamentParticipation(id);
                _ = await service.CreateAsync(participation, cancellationToken);

                var response = participation.MapToResponse();
                return TypedResults.CreatedAtRoute(
                    response,
                    GetTournamentByIdEndpoint.Name,
                    new { id = response.TournamentId });
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