using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Endpoints.MemberTypes;

public static class UpdateMemberTypeEndpoint
{
    public const string Name = "UpdateMemberType";

    public static IEndpointRouteBuilder MapUpdateMemberType(this IEndpointRouteBuilder app)
    {
        app.MapPut(GolfApiEndpoints.MemberTypes.Update, async (
                int id,
                UpdateMemberTypeRequest request,
                IMemberTypeService service,
                CancellationToken token = default) =>
            {
                var memberType = request.MapToMemberType(id);
                var updatedMemberType = await service.UpdateAsync(memberType, token);
                if (updatedMemberType is null)
                {
                    return Results.NotFound();
                }
                var response = updatedMemberType.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.MemberTypes.Tag)
            .Accepts<UpdateMemberTypeRequest>(isOptional: false, contentType: "application/json")
            .Produces<MemberTypeResponse>(contentType: "application/json")
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest);
        return app;
    }
}