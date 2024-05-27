namespace GolfLeague.Api.Endpoints.TournamentParticipations;

public static class TournamentParticipationEndpointsMapper
{
    public static IEndpointRouteBuilder MapTournamentParticipationEndpoints(this IEndpointRouteBuilder app)
    {
        return app
            .MapCreateTournamentParticipation()
            .MapGetTournamentParticipationById()
            .MapDeleteTournamentParticipation();
    }
}