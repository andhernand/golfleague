using GolfLeague.Api.Auth;
using GolfLeague.Application.Services;

using Microsoft.AspNetCore.Http.HttpResults;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class DeleteGolferEndpoint
{
    private const string Name = "DeleteGolfer";

    public static void MapDeleteGolfer(this IEndpointRouteBuilder app)
    {
        app.MapDelete(GolfApiEndpoints.Golfers.Delete,
                async Task<Results<NoContent, NotFound>> (
                    int id,
                    IGolferService service,
                    CancellationToken token = default) =>
                {
                    var deleted = await service.DeleteByIdAsync(id, token);

                    return deleted
                        ? TypedResults.NoContent()
                        : TypedResults.NotFound();
                })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .RequireAuthorization(AuthConstants.AdminPolicyName)
            .WithOpenApi();
    }
}