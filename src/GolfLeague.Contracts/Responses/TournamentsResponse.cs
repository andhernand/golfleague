namespace GolfLeague.Contracts.Responses;

public class TournamentsResponse
{
    public required IEnumerable<TournamentResponse> Tournaments { get; init; } = [];
}