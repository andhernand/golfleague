using GolfLeague.Api.Auth;
using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class UpdateGolferEndpoint
{
    private const string Name = "UpdateGolfer";

    public static IEndpointRouteBuilder MapUpdateGolfer(this IEndpointRouteBuilder app)
    {
        app.MapPut(GolfApiEndpoints.Golfers.Update, async (
                int id,
                UpdateGolferRequest request,
                IGolferService service,
                CancellationToken token = default) =>
            {
                var golfer = request.MapToGolfer(id);
                var updatedGolfer = await service.UpdateAsync(golfer, token);
                if (updatedGolfer is null)
                {
                    return Results.Problem(statusCode: StatusCodes.Status404NotFound);
                }

                var response = updatedGolfer.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .Accepts<UpdateGolferRequest>(isOptional: false, contentType: "application/json")
            .Produces<GolferResponse>(contentType: "application/json")
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound, contentType: "application/problem+json")
            .Produces<ValidationProblemDetails>(
                StatusCodes.Status400BadRequest,
                contentType: "application/problem+json")
            .RequireAuthorization(AuthConstants.TrustedPolicyName);

        return app;
    }
}