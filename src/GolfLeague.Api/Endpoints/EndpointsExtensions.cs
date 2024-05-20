using GolfLeague.Api.Endpoints.Members;
using GolfLeague.Api.Endpoints.MemberTypes;

namespace GolfLeague.Api.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        return app
            .MapMemberTypesEndpoints()
            .MapMemberEndpoints();
    }
}