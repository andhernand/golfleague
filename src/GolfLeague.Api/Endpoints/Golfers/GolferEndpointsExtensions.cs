namespace GolfLeague.Api.Endpoints.Golfers;

public static class GolferEndpointsExtensions
{
    public static IEndpointRouteBuilder MapGolferEndpoints(this IEndpointRouteBuilder app)
    {
        return app
            .MapCreateGolfer()
            .MapGetGolferById()
            .MapGetAllGolfers()
            .MapDeleteGolfer()
            .MapUpdateGolfer();
    }
}