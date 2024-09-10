namespace GolfLeague.Contracts.Requests;

public record CreateTournamentRequest
{
    public required string Name { get; init; }
    public required string Format { get; init; }
}