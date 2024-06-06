namespace GolfLeague.Contracts.Requests;

public class CreateTournamentGolferParticipationRequest
{
    public required int GolferId { get; init; }
    public required int Year { get; init; }
    public int? Score { get; init; }
}