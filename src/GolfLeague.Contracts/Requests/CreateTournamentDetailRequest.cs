namespace GolfLeague.Contracts.Requests;

public class CreateTournamentDetailRequest
{
    public required int GolferId { get; init; }
    public required int Year { get; init; }
    public int? Score { get; init; }
}