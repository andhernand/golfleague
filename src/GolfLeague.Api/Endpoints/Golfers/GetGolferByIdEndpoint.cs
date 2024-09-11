using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Http.HttpResults;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class GetGolferByIdEndpoint
{
    public const string Name = "GetGolferById";

    public static void MapGetGolferById(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.Golfers.Get,
                async Task<Results<Ok<GolferResponse>, NotFound>> (
                    int id,
                    IGolferService service,
                    CancellationToken token) =>
                {
                    var golfer = await service.GetGolferByIdAsync(id, token);

                    return golfer is null
                        ? TypedResults.NotFound()
                        : TypedResults.Ok(golfer);
                })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .RequireAuthorization()
            .WithOpenApi();
    }
}