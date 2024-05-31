using GolfLeague.Api.Auth;
using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

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
                    return Results.NotFound();
                }

                var response = updatedGolfer.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .Accepts<UpdateGolferRequest>(false, "application/json")
            .Produces<GolferResponse>(contentType: "application/json")
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(AuthConstants.TrustedPolicyName);

        return app;
    }
}