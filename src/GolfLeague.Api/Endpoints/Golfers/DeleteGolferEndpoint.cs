using GolfLeague.Application.Services;

using SerilogTimings;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class DeleteGolferEndpoint
{
    public const string Name = "DeleteGolfer";

    public static IEndpointRouteBuilder MapDeleteGolfer(this IEndpointRouteBuilder app)
    {
        app.MapDelete(GolfApiEndpoints.Golfers.Delete, async (
                int id,
                IGolferService service,
                CancellationToken token = default) =>
            {
                using var timedOperation = Operation.Begin("Delete Golfer");

                var deleted = await service.DeleteByIdAsync(id, token);

                timedOperation.Complete();
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}