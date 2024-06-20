using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class GetAllGolfersEndpoint
{
    private const string Name = "GetAllGolfers";

    public static void MapGetAllGolfers(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.Golfers.GetAll, async (
                IGolferService service,
                CancellationToken token) =>
            {
                var golfers = await service.GetAllGolfersAsync(token);
                var response = golfers.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .Produces<GolfersResponse>(contentType: "application/json")
            .RequireAuthorization();
    }
}