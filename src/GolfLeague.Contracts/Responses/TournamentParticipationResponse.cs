namespace GolfLeague.Contracts.Responses;

public class TournamentParticipationResponse
{
    public required int GolferId { get; init; }
    public required int TournamentId { get; init; }
    public required int Year { get; init; }
    public int? Score { get; init; }
}