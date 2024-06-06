namespace GolfLeague.Contracts.Requests;

public class UpdateGolferTournamentParticipationRequest
{
    public required int OriginalTournamentId { get; init; }
    public required int OriginalYear { get; init; }
    public required int NewTournamentId { get; init; }
    public required int NewYear { get; init; }
    public int? NewScore { get; init; }
}