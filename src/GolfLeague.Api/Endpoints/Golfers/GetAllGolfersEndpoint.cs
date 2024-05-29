using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

using SerilogTimings;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class GetAllGolfersEndpoint
{
    private const string Name = "GetAllGolfers";

    public static IEndpointRouteBuilder MapGetAllGolfers(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.Golfers.GetAll, async (
                IGolferService service,
                CancellationToken token) =>
            {
                using var timedOperation = Operation.Begin("Get All Golfers");

                var golfers = await service.GetAllGolfersAsync(token);
                var response = golfers.MapToResponse();

                timedOperation.Complete();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .Produces<GolfersResponse>(contentType: "application/json");

        return app;
    }
}