using GolfLeague.Api.Auth;
using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class CreateGolferEndpoint
{
    private const string Name = "CreateGolfer";

    public static void MapCreateGolfer(this IEndpointRouteBuilder app)
    {
        app.MapPost(GolfApiEndpoints.Golfers.Create, async (
                CreateGolferRequest request,
                IGolferService service,
                CancellationToken token = default) =>
            {
                var golfer = request.MapToGolfer();
                var createdId = await service.CreateAsync(golfer, token);
                golfer.GolferId = createdId;

                var response = golfer.MapToResponse();
                return TypedResults.CreatedAtRoute(
                    response,
                    GetGolferByIdEndpoint.Name,
                    new { id = golfer.GolferId });
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .Accepts<CreateGolferRequest>(isOptional: false, contentType: "application/json")
            .Produces<GolferResponse>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(
                StatusCodes.Status400BadRequest,
                contentType: "application/problem+json")
            .RequireAuthorization(AuthConstants.TrustedPolicyName);
    }
}