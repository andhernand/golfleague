using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Endpoints.Members;

public static class GetAllMembersEndpoint
{
    private const string Name = "GetAllMembers";

    public static IEndpointRouteBuilder MapGetAllMembers(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.Members.GetAll, async (IMemberService service, CancellationToken token) =>
        {
            var members = await service.GetAllMembersAsync(token);
            var response = members.MapToResponse();
            return TypedResults.Ok(response);
        })
        .WithName(Name)
        .WithTags(GolfApiEndpoints.Members.Tag)
        .Produces<MembersResponse>(contentType: "application/json");

        return app;
    }
}