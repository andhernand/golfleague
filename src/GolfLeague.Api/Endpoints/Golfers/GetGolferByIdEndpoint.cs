using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

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
                var golfer = await service.GetGolferByIdAsync(id, token);
                if (golfer is null)
                {
                    return Results.NotFound();
                }

                var response = golfer.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .Produces<GolferResponse>(contentType: "application/json")
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}