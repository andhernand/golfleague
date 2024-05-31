namespace GolfLeague.Application.Models;

public class TournamentParticipation
{
    public required int GolferId { get; init; }
    public required int TournamentId { get; init; }
    public required int Year { get; init; }
}