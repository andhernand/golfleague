using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Http.HttpResults;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class GetAllGolfersEndpoint
{
    private const string Name = "GetAllGolfers";

    public static void MapGetAllGolfers(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.Golfers.GetAll,
                async Task<Ok<IEnumerable<GolferResponse>>> (
                    IGolferService service,
                    CancellationToken token) =>
                {
                    var golfers = await service.GetAllGolfersAsync(token);
                    return TypedResults.Ok(golfers);
                })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .RequireAuthorization()
            .WithOpenApi();
    }
}