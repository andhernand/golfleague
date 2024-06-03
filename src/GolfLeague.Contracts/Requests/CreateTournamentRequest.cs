namespace GolfLeague.Contracts.Requests;

public class CreateTournamentRequest
{
    public required string Name { get; init; }
    public required string Format { get; init; }
}