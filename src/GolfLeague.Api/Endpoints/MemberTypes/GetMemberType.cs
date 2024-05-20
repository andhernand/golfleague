using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Endpoints.MemberTypes;

public static class GetMemberType
{
    public const string Name = "GetMemberType";

    public static IEndpointRouteBuilder MapGetMemberType(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.MemberTypes.Get, async (
                int id,
                IMemberTypeService service,
                CancellationToken token = default) =>
            {
                var memberType = await service.GetMemberTypeByIdAsync(id, token);
                if (memberType is null)
                {
                    return Results.NotFound();
                }

                var response = memberType.MapToResponse();
                return Results.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.MemberTypes.Tag)
            .Produces<MemberTypeResponse>(contentType: "application/json")
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}