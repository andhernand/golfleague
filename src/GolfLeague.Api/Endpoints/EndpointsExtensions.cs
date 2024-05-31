using GolfLeague.Api.Endpoints.Golfers;
using GolfLeague.Api.Endpoints.TournamentParticipations;
using GolfLeague.Api.Endpoints.Tournaments;

// ReSharper disable UnusedMethodReturnValue.Global

namespace GolfLeague.Api.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        return app
            .MapGolferEndpoints()
            .MapTournamentEndpoints()
            .MapTournamentParticipationEndpoints();
    }
}