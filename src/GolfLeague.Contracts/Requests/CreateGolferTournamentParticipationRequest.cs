namespace GolfLeague.Contracts.Requests;

public class CreateGolferTournamentParticipationRequest
{
    public required int TournamentId { get; init; }
    public required int Year { get; init; }
}