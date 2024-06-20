using GolfLeague.Api.Endpoints.Golfers;
using GolfLeague.Api.Endpoints.TournamentParticipation;
using GolfLeague.Api.Endpoints.Tournaments;

namespace GolfLeague.Api.Endpoints;

public static class EndpointsExtensions
{
    public static void MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGolferEndpoints();
        app.MapTournamentEndpoints();
        app.MapTournamentParticipationEndpoints();
    }
}