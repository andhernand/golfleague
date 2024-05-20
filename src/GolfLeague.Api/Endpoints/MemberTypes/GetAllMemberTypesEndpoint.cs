using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Endpoints.MemberTypes;

public static class GetAllMemberTypesEndpoint
{
    public const string Name = "GetAllMemberTypes";

    public static IEndpointRouteBuilder MapGetAllMembersTypes(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.MemberTypes.GetAll, async (
                IMemberTypeService service,
                CancellationToken token = default) =>
            {
                var memberTypes = await service.GetAllMemberTypesAsync(token);
                var response = memberTypes.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.MemberTypes.Tag)
            .Produces<MemberTypesResponse>(contentType: "application/json");

        return app;
    }
}