using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Endpoints.Members;

public static class GetMemberByIdEndpoint
{
    public const string Name = "GetMemberById";

    public static IEndpointRouteBuilder MapGetMemberById(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.Members.Get, async (
                int id,
                IMemberService service,
                CancellationToken token) =>
            {
                var member = await service.GetMemberByIdAsync(id, token);
                if (member is null)
                {
                    return Results.NotFound();
                }

                var response = member.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Members.Tag)
            .Produces<MemberResponse>(contentType: "application/json")
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}