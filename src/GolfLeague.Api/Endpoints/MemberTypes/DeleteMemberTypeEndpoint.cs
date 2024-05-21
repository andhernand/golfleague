using GolfLeague.Application.Services;

namespace GolfLeague.Api.Endpoints.MemberTypes;

public static class DeleteMemberTypeEndpoint
{
    public const string Name = "DeleteMemberType";

    public static IEndpointRouteBuilder MapDeleteMemberType(this IEndpointRouteBuilder app)
    {
        app.MapDelete(GolfApiEndpoints.MemberTypes.Delete, async (
                int id,
                IMemberTypeService service,
                CancellationToken token = default) =>
            {
                var deleted = await service.DeleteByIdAsync(id, token);
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.MemberTypes.Tag)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }
}