namespace GolfLeague.Api.Endpoints.TournamentParticipation;

public static class TournamentParticipationEndpointsMapper
{
    public static void MapTournamentParticipationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapDeleteTournamentParticipationEndpoint();
        app.MapParticipationDetailEndpoint();
        app.MapCreateTournamentDetailEndpoint();
    }
}