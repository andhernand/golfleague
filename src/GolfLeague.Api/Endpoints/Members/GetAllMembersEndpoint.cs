using GolfLeague.Api.Mapping;
using GolfLeague.Application.Models;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Endpoints.Members;

public static class GetAllMembersEndpoint
{
    public static IEndpointRouteBuilder MapGetAllMembers(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.Members.GetAllMembers, async (IMemberService service, CancellationToken token) =>
        {
            IEnumerable<Member> members = await service.GetAllMembersAsync(token);
            MembersResponse response = members.MapToResponse();
            return TypedResults.Ok(response);
        }).Produces<MembersResponse>(contentType: "application/json");

        return app;
    }
}