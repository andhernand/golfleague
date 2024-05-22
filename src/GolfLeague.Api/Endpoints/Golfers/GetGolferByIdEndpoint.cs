using GolfLeague.Application.Services;
using GolfLeague.Contracts.Responses;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class GetGolferByIdEndpoint
{
    public const string Name = "GetGolferById";

    public static IEndpointRouteBuilder MapGetGolferById(this IEndpointRouteBuilder app)
    {
        app.MapGet(GolfApiEndpoints.Golfers.Get, async (
                int id,
                IGolferService service,
                CancellationToken token) =>
            {
                // var member = await service.GetMemberByIdAsync(id, token);
                // if (member is null)
                // {
                //     return Results.NotFound();
                // }
                //
                // var response = member.MapToResponse();
                // return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .Produces<GolferResponse>(contentType: "application/json")
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}