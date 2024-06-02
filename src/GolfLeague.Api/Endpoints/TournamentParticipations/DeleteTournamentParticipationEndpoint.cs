using GolfLeague.Api.Auth;
using GolfLeague.Application.Models;
using GolfLeague.Application.Services;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Endpoints.TournamentParticipations;

public static class DeleteTournamentParticipationEndpoint
{
    private const string Name = "DeleteTournamentParticipation";

    public static IEndpointRouteBuilder MapDeleteTournamentParticipation(this IEndpointRouteBuilder app)
    {
        app.MapDelete(GolfApiEndpoints.TournamentParticipation.Delete, async (
                [FromQuery] int golferId,
                [FromQuery] int tournamentId,
                [FromQuery] int year,
                ITournamentParticipationService service,
                CancellationToken cancellationToken = default) =>
            {
                var id = new TournamentParticipation { GolferId = golferId, TournamentId = tournamentId, Year = year };
                var deleted = await service.DeleteAsync(id, cancellationToken);
                return deleted ? Results.NoContent() : Results.Problem(statusCode: StatusCodes.Status404NotFound);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.TournamentParticipation.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound, contentType: "application/problem+json")
            .RequireAuthorization(AuthConstants.AdminPolicyName);

        return app;
    }
}