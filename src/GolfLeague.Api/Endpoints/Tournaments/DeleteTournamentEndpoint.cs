using GolfLeague.Api.Auth;
using GolfLeague.Application.Services;

using Microsoft.AspNetCore.Http.HttpResults;

namespace GolfLeague.Api.Endpoints.Tournaments;

public static class DeleteTournamentEndpoint
{
    private const string Name = "DeleteTournament";

    public static void MapDeleteTournament(this IEndpointRouteBuilder app)
    {
        app.MapDelete(GolfApiEndpoints.Tournaments.Delete,
                async Task<Results<NoContent, NotFound>> (
                    int id,
                    ITournamentService service,
                    CancellationToken token = default) =>
                {
                    var deleted = await service.DeleteByIdAsync(id, token);

                    return deleted
                        ? TypedResults.NoContent()
                        : TypedResults.NotFound();
                })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .RequireAuthorization(AuthConstants.AdminPolicyName)
            .WithOpenApi();
    }
}