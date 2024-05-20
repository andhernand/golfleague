namespace GolfLeague.Api.Endpoints.MemberTypes;

public static class MemberTypesEndpointsExtensions
{
    public static IEndpointRouteBuilder MapMemberTypesEndpoints(this IEndpointRouteBuilder app)
    {
        return app
            .MapCreateMemberType()
            .MapGetMemberType();
    }
}