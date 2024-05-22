namespace GolfLeague.Api.Endpoints.Golfers;

public static class GolferEndpointsExtensions
{
    public static IEndpointRouteBuilder MapGolferEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateGolfer();
        return app;
    }
}