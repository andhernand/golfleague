namespace GolfLeague.Api.Endpoints.Tournaments;

public static class TournamentEndpointsMapper
{
    public static void MapTournamentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateTournament();
        app.MapGetTournamentById();
        app.MapGetAllTournaments();
        app.MapUpdateTournament();
        app.MapDeleteTournament();
    }
}