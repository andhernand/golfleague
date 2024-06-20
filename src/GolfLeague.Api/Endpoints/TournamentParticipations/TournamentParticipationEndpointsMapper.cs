namespace GolfLeague.Api.Endpoints.TournamentParticipations;

public static class TournamentParticipationEndpointsMapper
{
    public static void MapTournamentParticipationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetTournamentParticipationById();
        app.MapDeleteTournamentParticipation();
        app.MapCreateGolferTournamentParticipation();
        app.MapCreateTournamentGolferParticipationEndpoint();
        app.MapUpdateGolferTournamentParticipation();
        app.MapUpdateTournamentGolferParticipation();
    }
}