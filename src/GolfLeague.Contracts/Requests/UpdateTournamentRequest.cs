namespace GolfLeague.Contracts.Requests;

public record UpdateTournamentRequest
{
    public required int TournamentId { get; init; }
    public required string Name { get; init; }
    public required string Format { get; init; }
}