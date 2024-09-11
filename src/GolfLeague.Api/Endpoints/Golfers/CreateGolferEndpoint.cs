using GolfLeague.Api.Auth;
using GolfLeague.Api.Endpoints.Filters;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Http.HttpResults;

namespace GolfLeague.Api.Endpoints.Golfers;

public static class CreateGolferEndpoint
{
    private const string Name = "CreateGolfer";

    public static void MapCreateGolfer(this IEndpointRouteBuilder app)
    {
        app.MapPost(GolfApiEndpoints.Golfers.Create,
                async Task<Results<CreatedAtRoute<GolferResponse>, ValidationProblem>> (
                    CreateGolferRequest request,
                    IGolferService service,
                    CancellationToken token = default) =>
                {
                    var golfer = await service.CreateAsync(request, token);

                    return TypedResults.CreatedAtRoute(
                        golfer,
                        GetGolferByIdEndpoint.Name,
                        new { id = golfer.GolferId });
                })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Golfers.Tag)
            .Accepts<CreateGolferRequest>(isOptional: false, contentType: "application/json")
            .AddEndpointFilter<RequestValidationFilter<CreateGolferRequest>>()
            .RequireAuthorization(AuthConstants.TrustedPolicyName)
            .WithOpenApi();
    }
}