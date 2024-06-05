namespace GolfLeague.Application.Models;

public class UpdateTournamentParticipation
{
    public required TournamentParticipation Original { get; init; }
    public required TournamentParticipation Update { get; init; }
}