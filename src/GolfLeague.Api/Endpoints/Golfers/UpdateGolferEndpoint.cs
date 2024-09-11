using GolfLeague.Api.Auth;
using GolfLeague.Api.Endpoints.Filters;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Http.HttpResults;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class UpdateGolferEndpoint
{
    private const string Name = "UpdateGolfer";

    public static void MapUpdateGolfer(this IEndpointRouteBuilder app)
    {
        app.MapPut(GolfApiEndpoints.Golfers.Update,
                async Task<Results<Ok<GolferResponse>, NotFound, ValidationProblem>> (
                    int id,
                    UpdateGolferRequest request,
                    IGolferService service,
                    CancellationToken token = default) =>
                {
                    var golfer = await service.UpdateAsync(id, request, token);

                    return golfer is null
                        ? TypedResults.NotFound()
                        : TypedResults.Ok(golfer);
                })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .Accepts<UpdateGolferRequest>(isOptional: false, contentType: "application/json")
            .AddEndpointFilter<RequestValidationFilter<UpdateGolferRequest>>()
            .RequireAuthorization(AuthConstants.TrustedPolicyName)
            .WithOpenApi();
    }
}