namespace GolfLeague.Contracts.Requests;

public record CreateTournamentDetailRequest
{
    public required int GolferId { get; init; }
    public required int Year { get; init; }
    public int? Score { get; init; }
}