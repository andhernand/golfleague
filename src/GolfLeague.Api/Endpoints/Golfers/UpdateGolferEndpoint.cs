using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using SerilogTimings;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class UpdateGolferEndpoint
{
    public const string Name = "UpdateGolfer";

    public static IEndpointRouteBuilder MapUpdateGolfer(this IEndpointRouteBuilder app)
    {
        app.MapPut(GolfApiEndpoints.Golfers.Update, async (
                int id,
                UpdateGolferRequest request,
                IGolferService service,
                CancellationToken token = default) =>
            {
                using var timedOperation = Operation.Begin("Update Golfer");

                var golfer = request.MapToGolfer(id);
                var updatedGolfer = await service.UpdateAsync(golfer, token);
                if (updatedGolfer is null)
                {
                    timedOperation.Complete();
                    return Results.NotFound();
                }

                var response = updatedGolfer.MapToResponse();

                timedOperation.Complete();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .Accepts<UpdateGolferRequest>(false, "application/json")
            .Produces<GolferResponse>(contentType: "application/json")
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest);

        return app;
    }
}