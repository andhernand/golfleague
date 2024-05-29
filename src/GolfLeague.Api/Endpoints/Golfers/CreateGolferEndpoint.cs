using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using SerilogTimings;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class CreateGolferEndpoint
{
    public const string Name = "CreateGolfer";

    public static IEndpointRouteBuilder MapCreateGolfer(this IEndpointRouteBuilder app)
    {
        app.MapPost(GolfApiEndpoints.Golfers.Create, async (
                CreateGolferRequest request,
                IGolferService service,
                CancellationToken token = default) =>
            {
                using var timedOperation = Operation.Begin("Create Golfer");

                var golfer = request.MapToGolfer();
                var createdId = await service.CreateAsync(golfer, token);
                golfer.GolferId = createdId;
                var response = golfer.MapToResponse();

                timedOperation.Complete();

                return TypedResults.CreatedAtRoute(
                    response,
                    GetGolferByIdEndpoint.Name,
                    new { id = golfer.GolferId });
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .Accepts<CreateGolferRequest>(isOptional: false, contentType: "application/json")
            .Produces<GolferResponse>(StatusCodes.Status201Created)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest);

        return app;
    }
}