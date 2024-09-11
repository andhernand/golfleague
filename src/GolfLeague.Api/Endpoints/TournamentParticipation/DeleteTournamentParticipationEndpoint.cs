using GolfLeague.Api.Auth;
using GolfLeague.Application.Services;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Endpoints.TournamentParticipation;

public static class DeleteTournamentParticipationEndpoint
{
    private const string Name = "DeleteTournamentParticipation";

    public static void MapDeleteTournamentParticipationEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete(GolfApiEndpoints.TournamentParticipation.Delete,
                async Task<Results<NoContent, NotFound>> (
                    [FromQuery] int golferId,
                    [FromQuery] int tournamentId,
                    [FromQuery] int year,
                    ITournamentParticipationService service,
                    CancellationToken cancellationToken = default) =>
                {
                    var deleted = await service.DeleteAsync(golferId, tournamentId, year, cancellationToken);
                    return deleted
                        ? TypedResults.NoContent()
                        : TypedResults.NotFound();
                })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.TournamentParticipation.Tag)
            .RequireAuthorization(AuthConstants.AdminPolicyName)
            .WithOpenApi();
    }
}