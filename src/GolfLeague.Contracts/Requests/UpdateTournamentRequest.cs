namespace GolfLeague.Contracts.Requests;

public record UpdateTournamentRequest
{
    public required string Name { get; init; }
    public required string Format { get; init; }
}