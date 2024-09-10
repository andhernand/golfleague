namespace GolfLeague.Contracts.Requests;

public record CreateParticipationDetailRequest
{
    public required int TournamentId { get; init; }
    public required int Year { get; init; }
    public int? Score { get; init; }
}