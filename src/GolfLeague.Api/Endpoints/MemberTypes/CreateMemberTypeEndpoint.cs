using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Endpoints.MemberTypes;

public static class CreateMemberTypeEndpoint
{
    public const string Name = "CreateMemberType";

    public static IEndpointRouteBuilder MapCreateMemberType(this IEndpointRouteBuilder app)
    {
        app.MapPost(GolfApiEndpoints.MemberTypes.Create, async (
                CreateMemberTypeRequest request,
                IMemberTypeService service,
                CancellationToken token = default) =>
            {
                var memberType = request.MapToMemberType();
                var memberTypeId = await service.CreateAsync(memberType, token);
                memberType.MemberTypeId = memberTypeId;
                var response = memberType.MapToResponse();
                return TypedResults.CreatedAtRoute(
                    response,
                    GetMemberType.Name,
                    new { id = memberType.MemberTypeId });
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.MemberTypes.Tag)
            .Produces<MemberTypeResponse>(StatusCodes.Status201Created);

        return app;
    }
}