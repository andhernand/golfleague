﻿using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Endpoints.Members;

public static class CreateMemberEndpoint
{
    public const string Name = "CreateMember";

    public static IEndpointRouteBuilder MapCreateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPost(GolfApiEndpoints.Members.Create, async (
                CreateMemberRequest request,
                IMemberService service,
                CancellationToken token = default) =>
            {
                var member = request.MapToMember();
                var newMember = await service.Create(member, token);
                var response = newMember.MapToResponse();
                return TypedResults.CreatedAtRoute(
                    response,
                    GetMemberByIdEndpoint.Name,
                    new { id = newMember.MemberId });
            })
            .WithName(Name)
            .Produces<MemberResponse>(StatusCodes.Status201Created)
            .WithTags(GolfApiEndpoints.Members.Tag);

        return app;
    }
}