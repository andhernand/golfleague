using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Endpoints.Members;

public static class CreateMemberEndpoint
{
    public const string Name = "CreateMember";

    public static IEndpointRouteBuilder MapCreateMember(this IEndpointRouteBuilder app)
    {
        app.MapPost(GolfApiEndpoints.Members.Create, async (
                CreateMemberRequest request,
                IMemberService service,
                CancellationToken token = default) =>
            {
                var member = request.MapToMember();
                var createdId = await service.CreateAsync(member, token);
                member.MemberId = createdId;
                var response = member.MapToResponse();
                return TypedResults.CreatedAtRoute(
                    response,
                    GetMemberByIdEndpoint.Name,
                    new { id = member.MemberId });
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Members.Tag)
            .Accepts<CreateMemberRequest>(isOptional: false, contentType: "application/json")
            .Produces<MemberResponse>(StatusCodes.Status201Created)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest);

        return app;
    }
}