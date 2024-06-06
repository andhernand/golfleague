namespace GolfLeague.Contracts.Responses;

public class TournamentDetailResponse
{
    public required int TournamentId { get; init; }
    public required string Name { get; init; }
    public required string Format { get; init; }
    public required int Year { get; init; }
    public int? Score { get; init; }
}