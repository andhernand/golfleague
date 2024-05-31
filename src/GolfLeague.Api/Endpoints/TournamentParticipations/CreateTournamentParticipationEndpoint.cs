using GolfLeague.Api.Auth;
using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Endpoints.TournamentParticipations;

public static class CreateTournamentParticipationEndpoint
{
    private const string Name = "CreateTournamentParticipation";

    public static IEndpointRouteBuilder MapCreateTournamentParticipation(this IEndpointRouteBuilder app)
    {
        app.MapPost(GolfApiEndpoints.TournamentParticipation.Create, async (
                CreateTournamentParticipationsRequest request,
                ITournamentParticipationService service,
                CancellationToken token = default) =>
            {
                var tournamentParticipation = request.MapToTournamentParticipation();
                await service.CreateAsync(tournamentParticipation, token);

                var response = tournamentParticipation.MapToResponse();
                return TypedResults.CreatedAtRoute(
                    response,
                    GetTournamentParticipationByIdEndpoint.Name,
                    new { golferId = response.GolferId, tournamentId = response.TournamentId, year = response.Year });
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.TournamentParticipation.Tag)
            .Produces<TournamentParticipationResponse>(StatusCodes.Status201Created)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(AuthConstants.TrustedPolicyName);

        return app;
    }
}