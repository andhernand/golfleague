using GolfLeague.Api.Auth;
using GolfLeague.Application.Services;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Endpoints.Tournaments;

public static class DeleteTournamentEndpoint
{
    private const string Name = "DeleteTournament";

    public static void MapDeleteTournament(this IEndpointRouteBuilder app)
    {
        app.MapDelete(GolfApiEndpoints.Tournaments.Delete, async (
                int id,
                ITournamentService service,
                CancellationToken token = default) =>
            {
                var deleted = await service.DeleteByIdAsync(id, token);
                return deleted ? Results.NoContent() : Results.Problem(statusCode: StatusCodes.Status404NotFound);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound, contentType: "application/problem+json")
            .RequireAuthorization(AuthConstants.AdminPolicyName);
    }
}