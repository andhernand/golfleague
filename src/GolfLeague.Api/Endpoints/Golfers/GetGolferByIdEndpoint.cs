using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

using SerilogTimings;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class GetGolferByIdEndpoint
{
    public const string Name = "GetGolferById";

    public static IEndpointRouteBuilder MapGetGolferById(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.Golfers.Get, async (
                int id,
                IGolferService service,
                CancellationToken token) =>
            {
                using var timedOperation = Operation.Begin("Get Golfer By Id");

                var golfer = await service.GetGolferByIdAsync(id, token);
                if (golfer is null)
                {
                    timedOperation.Complete();
                    return Results.NotFound();
                }

                var response = golfer.MapToResponse();

                timedOperation.Complete();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .Produces<GolferResponse>(contentType: "application/json")
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        return app;
    }
}