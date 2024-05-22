using GolfLeague.Api.Endpoints.Golfers;

namespace GolfLeague.Api.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        return app.MapGolferEndpoints();
    }
}