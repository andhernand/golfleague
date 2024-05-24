namespace GolfLeague.Api.Endpoints.Tournaments;

public static class TournamentEndpointsMapper
{
    public static IEndpointRouteBuilder MapTournamentEndpoints(this IEndpointRouteBuilder app)
    {
        return app
            .MapCreateTournament()
            .MapGetTournamentById()
            .MapGetAllTournaments()
            .MapUpdateTournament()
            .MapDeleteTournament();
    }
}