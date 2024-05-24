namespace GolfLeague.Contracts.Responses;

public class TournamentResponse
{
    public required int TournamentId { get; init; }
    public required string Name { get; init; }
    public required string Format { get; init; }
}