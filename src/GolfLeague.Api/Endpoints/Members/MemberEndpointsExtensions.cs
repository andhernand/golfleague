﻿namespace GolfLeague.Api.Endpoints.Members;

public static class MemberEndpointsExtensions
{
    public static IEndpointRouteBuilder MapMemberEndpoints(this IEndpointRouteBuilder app)
    {
        return app
            .MapCreateMovie()
            .MapGetAllMembers()
            .MapGetMemberById();
    }
}