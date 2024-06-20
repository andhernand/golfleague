namespace GolfLeague.Api.Endpoints.Golfers;

public static class GolferEndpointsExtensions
{
    public static void MapGolferEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateGolfer();
        app.MapGetGolferById();
        app.MapGetAllGolfers();
        app.MapDeleteGolfer();
        app.MapUpdateGolfer();
    }
}