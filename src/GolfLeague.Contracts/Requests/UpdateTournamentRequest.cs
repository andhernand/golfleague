namespace GolfLeague.Contracts.Requests;

public class UpdateTournamentRequest
{
    public required string Name { get; init; }
    public required string Format { get; init; }
}