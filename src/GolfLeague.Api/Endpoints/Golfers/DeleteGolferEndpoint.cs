using GolfLeague.Api.Auth;
using GolfLeague.Application.Services;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class DeleteGolferEndpoint
{
    private const string Name = "DeleteGolfer";

    public static IEndpointRouteBuilder MapDeleteGolfer(this IEndpointRouteBuilder app)
    {
        app.MapDelete(GolfApiEndpoints.Golfers.Delete, async (
                int id,
                IGolferService service,
                CancellationToken token = default) =>
            {
                var deleted = await service.DeleteByIdAsync(id, token);
                return deleted ? Results.NoContent() : Results.Problem(statusCode: StatusCodes.Status404NotFound);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound, contentType: "application/problem+json")
            .RequireAuthorization(AuthConstants.AdminPolicyName);

        return app;
    }
}