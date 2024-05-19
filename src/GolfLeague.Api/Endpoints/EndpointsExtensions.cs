using GolfLeague.Api.Endpoints.Members;

namespace GolfLeague.Api.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapMemberEndpoints();
        return app;
    }
}