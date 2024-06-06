namespace GolfLeague.Contracts.Requests;

public class UpdateTournamentGolferParticipationRequest
{
    public required int OriginalGolferId { get; init; }
    public required int OriginalYear { get; init; }
    public required int NewGolferId { get; init; }
    public required int NewYear { get; init; }
    public int? NewScore { get; init; }
}