using GolfLeague.Api.Auth;
using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Endpoints.TournamentParticipations;

public static class CreateTournamentGolferParticipationEndpoint
{
    private const string Name = "CreateTournamentGolferParticipation";

    public static IEndpointRouteBuilder MapCreateTournamentGolferParticipationEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(GolfApiEndpoints.Tournaments.CreateTournamentGolferParticipations, async (
                int id,
                ITournamentParticipationService service,
                CreateTournamentGolferParticipationRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var participation = request.MapToTournamentParticipation(id);
                _ = await service.CreateAsync(participation, cancellationToken);

                var response = participation.MapToResponse();
                return TypedResults.CreatedAtRoute(
                    response,
                    GetTournamentParticipationByIdEndpoint.Name,
                    new { golferId = response.GolferId, tournamentId = response.TournamentId, year = response.Year });
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.TournamentParticipation.Tag)
            .Produces<TournamentParticipationResponse>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(
                StatusCodes.Status400BadRequest,
                contentType: "application/problem+json")
            .RequireAuthorization(AuthConstants.TrustedPolicyName);

        return app;
    }
}