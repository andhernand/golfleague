using GolfLeague.Api.Auth;
using GolfLeague.Api.Mapping;
using GolfLeague.Application.Services;
using GolfLeague.Contracts.Requests;
using GolfLeague.Contracts.Responses;

using Microsoft.AspNetCore.Mvc;

namespace GolfLeague.Api.Endpoints.Tournaments;

public static class CreateTournamentEndpoint
{
    private const string Name = "CreateTournament";

    public static IEndpointRouteBuilder MapCreateTournament(this IEndpointRouteBuilder app)
    {
        app.MapPost(GolfApiEndpoints.Tournaments.Create, async (
                CreateTournamentRequest request,
                ITournamentService service,
                CancellationToken token = default) =>
            {
                var tournament = request.MapToTournament();

                int createdId = await service.CreateAsync(tournament, token);
                tournament.TournamentId = createdId;

                var response = tournament.MapToResponse();

                return TypedResults.CreatedAtRoute(
                    response,
                    GetTournamentByIdEndpoint.Name,
                    new { id = tournament.TournamentId });
            })
            .WithName(Name)
            .WithTags(GolfApiEndpoints.Tournaments.Tag)
            .Accepts<CreateTournamentRequest>(false, "application/json")
            .Produces<TournamentResponse>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(
                StatusCodes.Status400BadRequest,
                contentType: "application/problem+json")
            .RequireAuthorization(AuthConstants.TrustedPolicyName);

        return app;
    }
}